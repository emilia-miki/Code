import random
from controller import Controller

people = ["mykytosyk", "vlad", "anna", "miki", "kate", "james", "elise"]
games = ["Demon's Souls", "Dark Souls", "Bloodborne", "Sekiro: Shadows Die Twice", "Elden Ring"]

controller = Controller("bolt://localhost:7687", "neo4j", "password")
controller.clear()

for person in people:
    controller.create_person(person)

for game in games:
    controller.create_game(game)

for person in people:
    number_of_likes = random.randint(1, 3)
    sample = random.sample(games, number_of_likes)
    for game in sample:
        controller.create_likes_relationship(person, game)

controller.all_paths(people)

controller.close()
