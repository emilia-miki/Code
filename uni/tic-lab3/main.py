import io
import os
import struct
import sys
from os.path import exists
from collections import defaultdict
from queue import PriorityQueue
from bitarray import bitarray
from tqdm import tqdm
import math


class Node:
    def __init__(self, byte: bytes, weight: int = None):
        self.left = None
        self.right = None
        self.byte = byte
        self.weight = weight

    def __eq__(self, other): return self.weight == other.weight
    def __ne__(self, other): return self.weight != other.weight
    def __lt__(self, other): return self.weight < other.weight
    def __le__(self, other): return self.weight <= other.weight
    def __gt__(self, other): return self.weight > other.weight
    def __ge__(self, other): return self.weight >= other.weight


def count_nodes(tree: Node) -> int:
    count = 1
    if tree.left is not None:
        count += count_nodes(tree.left)
    if tree.right is not None:
        count += count_nodes(tree.right)
    return count


def get_encodings(tree: Node, bits: bitarray = bitarray()) -> dict:
    encodings = dict()

    if tree.left is not None:
        updated = bitarray.copy(bits)
        updated.append(False)
        encodings.update(get_encodings(tree.left, updated))

    if tree.right is not None:
        updated = bitarray.copy(bits)
        updated.append(True)
        encodings.update(get_encodings(tree.right, updated))

    if tree.left is None and tree.right is None:
        encodings[tree.byte] = bits

    return encodings


def make_nodes_unique(tree: Node, depth: int = 0):
    if tree.left is None and tree.right is None:
        tree.weight = depth
    else:
        tree.byte = struct.pack('B', make_nodes_unique.byte)
        tree.weight = depth
        make_nodes_unique.byte += 1
        if make_nodes_unique.byte == 256:
            make_nodes_unique.byte = 0

        if tree.left is not None:
            make_nodes_unique(tree.left, depth + 1)

        if tree.right is not None:
            make_nodes_unique(tree.right, depth + 1)


def preorder(tree: Node) -> bytes:
    result = b''

    result += tree.byte + struct.pack('B', tree.weight)

    if tree.left is not None:
        result += preorder(tree.left)

    if tree.right is not None:
        result += preorder(tree.right)

    return result


def inorder(tree: Node) -> bytes:
    result = b''

    if tree.left is not None:
        result += inorder(tree.left)

    result += tree.byte + struct.pack('B', tree.weight)

    if tree.right is not None:
        result += inorder(tree.right)

    return result


def sum_of_bytes(dictionary: dict) -> int:
    result = 0
    for item in dictionary.values():
        result += item
    return result


def entropy(dictionary: dict) -> float:
    total = sum_of_bytes(dictionary)
    probabilities = dict()
    for key in dictionary.keys():
        probabilities[key] = dictionary[key] / total

    result = 0
    for key in probabilities.keys():
        p = probabilities[key]
        result += -p * math.log2(p)

    return result


def encode(file, out_file):
    print('Building the Huffman tree...')

    bytes_dictionary = defaultdict(lambda: 0)
    file.seek(0, os.SEEK_END)
    file_size_in_mb = file.tell() / 1048576
    file.seek(0)
    if file_size_in_mb > 10:
        pbar = tqdm(total=file_size_in_mb, unit='MB', unit_scale=True)
    bytes_batch: bytes
    while bytes_batch := file.read(256 * 1024):
        for next_byte in bytes_batch:
            bytes_dictionary[struct.pack('B', next_byte)] += 1
        if file_size_in_mb > 10:
            pbar.update(256 / 1024)
    if file_size_in_mb > 10:
        pbar.clear()
        pbar.close()

    print(f'Original file entropy is {entropy(bytes_dictionary)} bytes')

    # build the huffman tree
    q = PriorityQueue()
    for item in bytes_dictionary.items():
        q.put(Node(item[0], item[1]))

    while q.qsize() > 1:
        first_node = q.get()
        second_node = q.get()
        new_node = Node(b'\0', first_node.weight + second_node.weight)
        new_node.left = first_node
        new_node.right = second_node
        q.put(new_node)

    huffman_tree = q.get()
    make_nodes_unique(huffman_tree)
    encodings = get_encodings(huffman_tree)
    readable_encodings = dict()
    for key in encodings.keys():
        readable_encodings[key.decode('koi8-u')] = encodings[key]

    out_file.write(b'\0huf')
    count = count_nodes(huffman_tree)
    print(f'The Huffman tree contains {int(count)} nodes')
    out_file.write(struct.pack('i', 2 * count))
    out_file.write(preorder(huffman_tree))
    out_file.write(inorder(huffman_tree))
    bytes_sum = sum_of_bytes(bytes_dictionary)
    print(f'The original file size is {bytes_sum} bytes')
    print('Encoding the file...')
    out_file.write(struct.pack('i', bytes_sum))
    buffer = bitarray()
    file.seek(0)
    output_bytes_count = 0
    output_bytes = defaultdict(lambda: 0)
    if file_size_in_mb > 10:
        pbar = tqdm(total=file_size_in_mb, unit='MB', unit_scale=True)
    while bytes_batch := file.read(256 * 1024):
        for next_byte in bytes_batch:
            buffer.extend(encodings[struct.pack('B', next_byte)])
            if len(buffer) >= 8:
                encoded_byte = buffer[:8].tobytes()
                output_bytes[encoded_byte] += 1
                out_file.write(encoded_byte)
                buffer = buffer[8:]
                output_bytes_count += 1
        if file_size_in_mb > 10:
            pbar.update(len(bytes_batch) / 1048576)
    if file_size_in_mb > 10:
        pbar.clear()
        pbar.close()
    if len(buffer) > 0:
        output_bytes_count += 1
        for _ in range(8 - len(buffer)):
            buffer.append(False)
        output_bytes[buffer.tobytes()] += 1
    out_file.write(buffer.tobytes())
    print('File encoded')
    print(f'The encoded data takes up {output_bytes_count} bytes')
    print(f'Compression coefficient: {round(bytes_sum / output_bytes_count, 3)}')
    print(f'Encoded data entropy is {entropy(output_bytes)} bytes')


def build_tree(in_order) -> Node:
    if len(in_order) > 0:
        root = Node(build_tree.pre_order[:1], build_tree.pre_order[1])
        root_index = in_order.index(build_tree.pre_order[:2])
        while root_index % 2 != 0:
            root_index = in_order.index(
                build_tree.pre_order[:2], root_index + 1)
        build_tree.pre_order = build_tree.pre_order[2:]
        root.left = build_tree(in_order[:root_index])
        root.right = build_tree(in_order[root_index + 2:])
        return root


def decode(file, out_file):
    file.read(4)
    nodes_count = struct.unpack('i', file.read(4))[0]
    print(f'The Huffman tree contains {nodes_count / 2} nodes')
    pre_order = file.read(nodes_count)
    in_order = file.read(nodes_count)
    length = struct.unpack('i', file.read(4))[0]
    print(f'The decompressed file size will be {length} bytes')
    build_tree.pre_order = pre_order
    print('Constructing the Huffman tree...')
    huffman_tree = build_tree(in_order)

    print('Decoding the file...')
    node = huffman_tree
    file_size_in_mb = length / 1048576
    if file_size_in_mb > 10:
        pbar = tqdm(total=file_size_in_mb, unit='MB', unit_scale=True)
    buffer = bitarray()
    while bytes_batch := file.read(256 * 1024):
        for next_byte in bytes_batch:
            buffer.clear()
            buffer.frombytes(struct.pack('B', next_byte))
            for bit in buffer:
                if bit == 1:
                    node = node.right
                else:
                    node = node.left
                if node.left is None and node.right is None:
                    out_file.write(node.byte)
                    node = huffman_tree
                    length -= 1
                    if length == 0:
                        break
            if length == 0:
                break
        if file_size_in_mb > 10:
            pbar.update(256 / 1024)
    if file_size_in_mb > 10:
        pbar.clear()
        pbar.close()
    print('File decoded')


make_nodes_unique.byte = 0
build_tree.pre_order = None

response = input('Do you want to encode a file? (Y/n) ')
if response == '' or response.lower == 'y':
    filename = input('Enter file name: ')

    if not exists(filename):
        try:
            sys.exit(0)
        except SystemExit:
            print('The file does not exist.')

    f = open(filename, 'rb')
    magic = f.read(4)
    f.seek(0)
    if magic == b'\0huf':
        if filename.endswith('.encoded') and filename.count('.encoded') == 1:
            out_filename = filename.replace('.encoded', '')
        else:
            out_filename = filename + '.decoded'
        out_f = open(out_filename, 'wb')
        decode(f, out_f)
    else:
        out_f = open(filename + '.encoded', 'wb')
        encode(f, out_f)
    f.close()
    out_f.close()
else:
    text = input('Enter some text: ')
    out_f = open('text.encoded', 'wb')
    encode(io.BytesIO(bytes(text, 'utf-8')), out_f)
