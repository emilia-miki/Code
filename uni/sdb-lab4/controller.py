from neo4j import GraphDatabase
from itertools import combinations


class Controller:
    def __init__(self, uri, user, password):
        self.driver = GraphDatabase.driver(uri, auth=(user, password))

    def clear(self):
        with self.driver.session() as session:
            session.execute_write(self._clear)

    def close(self):
        self.driver.close()

    def create_person(self, person_name):
        with self.driver.session() as session:
            result = session.execute_write(self._create_and_return_person, person_name)
            for row in result:
                print("Created person: {p}".format(p=row['p']))

    def create_game(self, game_name):
        with self.driver.session() as session:
            result = session.execute_write(self._create_and_return_game, game_name)
            for row in result:
                print("Created game: {g}".format(g=row['g']))

    def create_likes_relationship(self, person_name, game_name):
        with self.driver.session() as session:
            result = session.execute_write(
                self._create_and_return_relationship, person_name, game_name)
            for row in result:
                print("Created relationship: {p} {r} {g}".format(p=row['p'], r=row['r'], g=row['g']))

    def all_paths(self, people):
        with self.driver.session() as session:
            for pair in combinations(people, 2):
                start_name, end_name = pair
                result = session.execute_read(self._find_shortest_path, start_name, end_name)
                if len(result) == 0:
                    print(f"A path between {start_name} and {end_name} does not exist")
                else:
                    path_length = result[0]["p"].count("likes")
                    print(f"A path between {start_name} and {end_name} has length {path_length}")

    @staticmethod
    def _find_shortest_path(tx, start_name, end_name):
        query = (
            "MATCH (start:Person {name: $start_name}), (end:Person {name: $end_name}), "
            "p = shortestPath((start)-[*]-(end)) "
            "RETURN p"
        )
        result = tx.run(query, start_name=start_name, end_name=end_name)
        return [row.data() for row in result]

    @staticmethod
    def _clear(tx):
        query = (
            "MATCH (n) "
            "DETACH DELETE n"
        )
        tx.run(query)

    @staticmethod
    def _create_and_return_person(tx, person_name):
        query = (
            "CREATE (p:Person { name: $person_name }) "
            "RETURN p"
        )
        result = tx.run(query, person_name=person_name)
        return [{"p": row["p"]["name"]} for row in result]

    @staticmethod
    def _create_and_return_game(tx, game_name):
        query = (
            "CREATE (g:Game { name: $game_name }) "
            "RETURN g"
        )
        result = tx.run(query, game_name=game_name)
        return [{"g": row["g"]["name"]} for row in result]

    @staticmethod
    def _create_and_return_relationship(tx, person_name, game_name):
        query = (
            "MATCH (p:Person { name: $person_name }), (g:Game { name: $game_name }) "
            "CREATE (p)-[r:likes]->(g) "
            "RETURN p, r, g"
        )
        result = tx.run(query, person_name=person_name, game_name=game_name)
        return [{"p": row["p"]["name"], "r": "likes", "g": row["g"]["name"]} for row in result]
