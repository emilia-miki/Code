import math
import os.path
import struct
import sys
import random

from rs import *

Galois.init_tables()
response = input("Do you want to encode data? (Y/n) ")
if response.lower() != "n":
    response = input("Do you want to enter text manually? (Y/n) ")
    nsym = int(input("How many errors do you want to be able to correct per 256 bits? "))
    errors = int(input("How many random errors do you want to add per 256 bits? "))
    with open("output_file", "wb") as f_out:
        if response.lower() != "n":
            text = input("Enter the text: ")
            data = [int(b) for b in text.encode("utf-8")]
            f_out.write(struct.pack('i', nsym))
            for i in range(math.ceil(len(data) / 255)):
                encoded = rs_encode_msg(data[i * 256:(i + 1) * 256], nsym)
                if errors > 0:
                    step = int(len(encoded) / errors)
                    for i in range(errors):
                        index = step * i + random.randint(0, step - 1)
                        encoded[index] = 255 - encoded[index]
                f_out.write(bytes(encoded))
        else:
            filename = input("Enter the name of the file to encode: ")
            if not os.path.exists(filename):
                print("This file does not exist")
                try:
                    sys.exit(0)
                except SystemExit:
                    pass
            f_out.write(struct.pack('i', nsym))
            with open(filename, "rb") as f:
                while bytes_batch := f.read(255 - nsym):
                    data = [int(b) for b in bytes_batch]
                    encoded = rs_encode_msg(data, nsym)
                    if errors > 0:
                        step = int(len(encoded) / errors)
                        for i in range(errors):
                            index = step * i + random.randint(0, step - 1)
                            encoded[index] = 255 - encoded[index]
                    f_out.write(bytes(encoded))
else:
    filename = input("Enter the name of the file to be decoded: ")
    if not os.path.exists(filename):
        print("This file does not exist")
        try:
            sys.exit(0)
        except SystemExit:
            pass
    with open(filename, "rb") as f:
        with open(filename + ".decoded", "wb") as f_out:
            nsym = struct.unpack('i', f.read(4))[0]
            while bytes_batch := f.read(255):
                corrected = rs_correct_msg([int(b) for b in bytes_batch], nsym)
                f_out.write(bytearray(corrected[0]))
