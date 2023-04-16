matrix = [
    [0, 1, 1],
    [0, 0, 1],
    [0, 0, 0]
]

estimations = [100, 80, 60]

print('Метод парних порівнянь')
print('\nМатриця бінарних уподобань:')
print('Zi Z1 Z2 Z3')
for i in range(len(matrix)):
    row = f'Z{i + 1} '
    for j in range(len(matrix[0])):
        if i == j:
            row += '   '
        else:
            row += f'{matrix[i][j]}  '
    print(row)

print('\nЦіна кожної мети:')
prices = []
for i in range(len(matrix)):
    prices.append(sum(matrix[i]))
    print(f'C{i + 1} = {sum(matrix[i])}')
total_price = sum(prices)

print('\nВага кожної мети:')
weights = []
for i in range(len(matrix)):
    weights.append(prices[i] / total_price)
    print(f'V{i + 1} = {round(weights[i], 2)}')

print(f'\nНайкраща альтернатива: {weights.index(max(weights)) + 1}')

print('\n\nМетод послідовних порівнянь')
print(f'\nПочаткові оцінки:')
for i, estimation in enumerate(estimations):
    print(f'p{i + 1} = {estimation}')

print('\nСкориговані оцінки:')
prices = estimations.copy()
for i in range(len(prices)):
    for j in range(i + 1, len(estimations)):
        for k in range(j + 1, len(estimations)):
            if prices[i] <= prices[j] + prices[k]:
                prices[i] = prices[j] + prices[k] + 10
total_price = sum(prices)

print('\nВага кожної мети:')
weights = []
for i, price in enumerate(prices):
    weights.append(price / total_price)
    print(f'V{i + 1} = {round(weights[i], 2)}')

print(f'\nНайкраща альтернатива – {weights.index(max(weights)) + 1}')
