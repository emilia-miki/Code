import io
import math
import os
import random
import sys
from os.path import exists
from bitarray import bitarray
import struct


def encode_batch(bits):
    one = sum(bits[i] for i in [0, 1, 3, 4, 6, 8, 10]) % 2
    two = sum(bits[i] for i in [0, 2, 3, 5, 6, 9, 10]) % 2
    three = sum(bits[i] for i in [1, 2, 3, 7, 8, 9, 10]) % 2
    four = sum(bits[i] for i in [4, 5, 6, 7, 8, 9, 10]) % 2
    zero = (one + two + three + four + sum(bits[:11])) % 2

    out_bits = bitarray()
    out_bits.append(zero)
    out_bits.append(one)
    out_bits.append(two)
    out_bits.append(bits[0])
    out_bits.append(three)
    out_bits.extend(bits[1:4])
    out_bits.append(four)
    out_bits.extend(bits[4:11])

    return out_bits


def encode(file, outfile, errors=None):
    file.seek(0, os.SEEK_END)
    length = file.tell()
    outfile.write(struct.pack('i', length))
    file.seek(0)
    bits = bitarray()
    byte_iter = 0
    error_index = 0
    introduced = 0
    while bytes_batch := file.read(256 * 1024):
        for byte in bytes_batch:
            bits.frombytes(struct.pack('B', byte))
            if len(bits) >= 11:
                out_bits = encode_batch(bits)
                if error_index < len(errors):
                    bytepos, bitpos = errors[error_index]
                    if 0 <= bitpos < 8:
                        if byte_iter == bytepos:
                            out_bits[bitpos] = 1 - out_bits[bitpos]
                            error_index += 1
                            introduced += 1
                            if error_index < len(errors):
                                bytepos, bitpos = errors[error_index]
                        if byte_iter + 1 == bytepos:
                            out_bits[8 + bitpos] = 1 - out_bits[8 + bitpos]
                            error_index += 1
                            introduced += 1
                byte_iter += 2
                bits = bits[11:]
                outfile.write(out_bits.tobytes())
                out_bits.clear()
    if len(bits) > 0:
        for _ in range(11 - len(bits)):
            bits.append(0)
        out_bits = encode_batch(bits)
        outfile.write(out_bits.tobytes())
    print(f'introduced {introduced} errors')


def decode(file, outfile):
    corrected = 0
    length = struct.unpack('i', file.read(4))[0]
    bits = bitarray()
    outbits = bitarray()
    while bytes_batch := file.read(256 * 1024):
        for byte in bytes_batch:
            bits.frombytes(struct.pack('B', byte))
            if len(bits) == 16:
                zero = sum(bits) % 2
                one = sum(bits[j] for j in [1, 3, 5, 7, 9, 11, 13, 15]) % 2
                two = sum(bits[j] for j in [2, 3, 6, 7, 10, 11, 14, 15]) % 2
                three = sum(bits[j] for j in [4, 5, 6, 7, 12, 13, 14, 15]) % 2
                four = sum(bits[j] for j in [8, 9, 10, 11, 12, 13, 14, 15]) % 2

                position = bitarray()
                for _ in range(4):
                    position.append(0)
                position.append(four)
                position.append(three)
                position.append(two)
                position.append(one)
                position = position.tobytes()[0]

                if position != 0 and zero == 0:
                    print('2 errors detected')

                if position != 0 and zero == 1:
                    corrected += 1
                    bits[position] = 1 - bits[position]

                if position == 0 and zero == 1:
                    corrected += 1
                    bits[position] = 1 - bits[position]

                outbits.append(bits[3])
                outbits.extend(bits[5:8])
                outbits.extend(bits[9:])
                for _ in range(2):
                    if length > 0 and len(outbits) >= 8:
                        outfile.write(outbits[:8].tobytes())
                        outbits = outbits[8:]
                        length -= 1
                if length == 0:
                    break
                bits.clear()
    print(f'{corrected} errors corrected')


response = input('Do you want to encode or decode? (E/d) ')
if response == 'd':
    filename = input('Enter the name of the file to decode: ')

    if not exists(filename):
        try:
            sys.exit(0)
        except SystemExit:
            print('The file does not exist.')

    f = open(filename, 'rb')
    if filename.endswith('.encoded') and filename.count('.encoded') == 1:
        out_f = open(filename.replace('.encoded', ''), 'wb')
    else:
        out_f = open(filename + '.decoded', 'wb')
    decode(f, out_f)
else:
    response = input('Do you want to encode a file? (Y/n) ')
    if response == '' or response.lower() == 'y':
        filename = input('Enter file name: ')

        if not exists(filename):
            try:
                sys.exit(0)
            except SystemExit:
                print('The file does not exist.')

        f = open(filename, 'rb')
        out_f = open(filename + '.encoded', 'wb')
    else:
        text = input('Enter the text: ')
        f = io.BytesIO(bytes(text, 'utf-8'))
        out_f = open('text.encoded', 'wb')

    f.seek(0, os.SEEK_END)
    orig_length = f.tell()
    f.seek(0)
    increased_length = math.ceil(orig_length * 16 / 11)
    response = input('How many errors do you want to introduce? ')
    number_of_errors = int(response)
    arr = []
    for i in random.sample(range(int(increased_length / 2)), number_of_errors):
        arr.append((i * 2 + random.randint(0, 1), random.randint(0, 7)))
    encode(f, out_f, sorted(arr))

f.close()
out_f.close()
