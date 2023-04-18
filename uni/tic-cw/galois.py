class ReedSolomonError(Exception):
    pass


class Galois:
    _exp = [0] * 512
    _log = [0] * 256

    @staticmethod
    def init_tables(prim=0x11d):
        x = 1
        for i in range(0, 255):
            Galois._exp[i] = x
            Galois._log[x] = i
            x = Galois.mul_noLUT(x, 2, prim)

        for i in range(255, 512):
            Galois._exp[i] = Galois._exp[i - 255]

        return [Galois._log, Galois._exp]

    @staticmethod
    def mul_noLUT(x, y, prim=0, field_charac_full=256, carryless=True):
        r = 0
        while y:
            if y & 1: r = r ^ x if carryless else r + x
            y = y >> 1
            x = x << 1
            if prim > 0 and x & field_charac_full: x = x ^ prim

        return r

    def __init__(self):
        Galois.init_tables()

    @staticmethod
    def add(x, y):
        return x ^ y

    @staticmethod
    def sub(x, y):
        return x ^ y

    @staticmethod
    def mul(x, y):
        if x == 0 or y == 0:
            return 0
        return Galois._exp[Galois._log[x] + Galois._log[y]]

    @staticmethod
    def div(x, y):
        if y == 0:
            raise ZeroDivisionError()
        if x == 0:
            return 0
        return Galois._exp[(Galois._log[x] + 255 - Galois._log[y]) % 255]

    @staticmethod
    def pow(x, power):
        return Galois._exp[(Galois._log[x] * power) % 255]

    @staticmethod
    def inverse(x):
        return Galois._exp[255 - Galois._log[x]]

    @staticmethod
    def poly_scale(p, x):
        r = [0] * len(p)

        for i in range(0, len(p)):
            r[i] = Galois.mul(p[i], x)

        return r

    @staticmethod
    def poly_add(p, q):
        r = [0] * max(len(p), len(q))

        for i in range(0, len(p)):
            r[i + len(r) - len(p)] = p[i]

        for i in range(0, len(q)):
            r[i + len(r) - len(q)] ^= q[i]

        return r

    @staticmethod
    def poly_mul(p, q):
        r = [0] * (len(p) + len(q) - 1)

        for j in range(0, len(q)):
            for i in range(0, len(p)):
                r[i + j] ^= Galois.mul(p[i], q[j])

        return r

    @staticmethod
    def poly_eval(poly, x):
        y = poly[0]

        for i in range(1, len(poly)):
            y = Galois.mul(y, x) ^ poly[i]

        return y

    @staticmethod
    def poly_div(dividend, divisor):
        msg_out = list(dividend)

        for i in range(0, len(dividend) - (len(divisor) - 1)):
            coef = msg_out[i]
            if coef != 0:
                for j in range(1, len(divisor)):
                    if divisor[j] != 0:
                        msg_out[i + j] ^= Galois.mul(divisor[j], coef)

        separator = -(len(divisor) - 1)
        return msg_out[:separator], msg_out[separator:]
