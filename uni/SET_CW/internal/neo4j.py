import neo4j.exceptions
from neo4j import GraphDatabase
from .models import *
from typing import Union

URI = "neo4j://localhost"
AUTH = ("neo4j", "admin")


class PartnersDB:
    def __init__(self):
        driver = GraphDatabase.driver(URI, auth=AUTH)
        driver.verify_connectivity()
        self.session = driver.session(database="neo4j")

        constrains_check_res = self.session.execute_read(PartnersDB._tr_pre_check_labels)[0]
        print("Constraint check found ", constrains_check_res, " sample instance(s).")
        if constrains_check_res < 2:
            self.p_constraint_create_res = self.session.execute_write(PartnersDB._tr_pre_create_constraint_p)
            self.w_constraint_create_res = self.session.execute_write(PartnersDB._tr_pre_create_constraint_w)
            self.labels_create_res = self.session.execute_write(PartnersDB._tr_pre_create_labels)

    @staticmethod
    def _tr_pre_check_labels(tx):
        result = tx.run("""SHOW CONSTRAINTS""")

        data = result.values("name")
        check = 0
        for piece in data:
            if piece[0] == "P_U" or piece[0] == "WA_U":
                check += 1

        summary = result.consume()
        return check, summary

    @staticmethod
    def _tr_pre_create_constraint_w(tx):
        result = tx.run("""CREATE CONSTRAINT WA_U IF NOT EXISTS
        FOR (w:FieldOfService)
        REQUIRE (w.name) IS UNIQUE """)
        records = list(result)
        summary = result.consume()
        return records, summary

    @staticmethod
    def _tr_pre_create_constraint_p(tx):
        result = tx.run("""CREATE CONSTRAINT P_U IF NOT EXISTS
        FOR  (p:Partner)
        REQUIRE (p.name) IS UNIQUE""")
        records = list(result)
        summary = result.consume()
        return records, summary

    @staticmethod
    def _tr_pre_create_labels(tx):
        result = tx.run("""CREATE
        (w0:FieldOfService{name: "Test0"}),
        (w1:FieldOfService{name: "Test1"}),
        (p0:Partner{name:"Test0"})
        CREATE
        (w0)-[:CONNECTS]->(w1),
        (p0)-[:RULES]->(w0)""")
        records = list(result)
        summary = result.consume()
        return records, summary

    # def _tr_Area_to_Area_conn(tx, start_name, end_name):
    #     result = tx.run("""MATCH (w0:FieldOfService{name: $area_0})
    #     MATCH (w1:FieldOfService{name:$area_1})
    #     MERGE (w0)-[:CONNECTS]->(w1)""", area_0=start_name, area_1=end_name)
    #     records = list(result)
    #     summary = result.consume()
    #     return records, summary

    # def settle_Area_to_Area_conn(self, start_name, end_name):
    #     records, summary = self.session.execute_write(PartnersDB._tr_Area_to_Area_conn,
    #                                                   start_name=start_name, end_name=end_name)
    #     if summary.counters.nodes_created < 1:
    #         return False
    #     else:
    #         return True

    @staticmethod
    def _tr_add_partner_to_fos_conn(tx, partner_name, fos_name):
        result = tx.run("""MATCH (w:FieldOfService{name: $fos})
        MATCH (p:Partner{name:$part})
        MERGE (p)-[:RULES]->(w)""", fos=fos_name, part=partner_name)
        records = list(result)
        summary = result.consume()
        return records, summary

    def add_partner_to_fos_conn(self, partner_name, fos_name):
        records, summary = self.session.execute_write(PartnersDB._tr_add_partner_to_fos_conn,
                                                      partner_name=partner_name, fos_name=fos_name)
        if summary.counters.nodes_created < 1:
            return False
        else:
            return True

    # def _tr_del_Area_to_Area_conn(tx, start_area_name, end_area_name):
    #     result = tx.run("""MATCH (:FieldOfService{name: $start_area})-[con:CONNECTS]-(:FieldOfService{name: $end_area})
    #     DELETE con""",
    #                     start_area=start_area_name,
    #                     end_area=end_area_name
    #                     )
    #     records = list(result)
    #     summary = result.consume()
    #     return records, summary

    # def del_Area_to_Area_conn(self, start_area_name, end_area_name):
    #     records, summary = self.session.execute_write(PartnersDB._tr_del_Area_to_Area_conn,
    #                                                   start_area_name=start_area_name,
    #                                                   end_area_name=end_area_name)
    #     if summary.counters.nodes_created < 1:
    #         return False
    #     else:
    #         return True

    @staticmethod
    def _tr_del_partner_to_fos_conn(tx, start_partner_name, end_area_name):
        result = tx.run("""MATCH (:Partner{name: $start_partner})-[con:RULES]-(:FieldOfService{name: $end_area})
        DELETE con""",
                        start_partner=start_partner_name,
                        end_area=end_area_name
                        )
        records = list(result)
        summary = result.consume()
        return records, summary

    def del_partner_to_fos_conn(self, start_partner_name, end_area_name):
        records, summary = self.session.execute_write(PartnersDB._tr_del_partner_to_fos_conn,
                                                      start_partner_name=start_partner_name,
                                                      end_area_name=end_area_name)
        if summary.counters.nodes_created < 1:
            return False
        else:
            return True

    @staticmethod
    def _tr_get_all_fields_of_service(tx):
        result = tx.run("""MATCH (a:FieldOfService) RETURN a""")
        records = list(result)

        names = []
        for piece in records:
            names.append(piece.data()['a']['name'])
        summary = result.consume()
        return names, summary

    def get_all_fields_of_service(self) -> list[FieldOfService]:
        names, summary = self.session.execute_write(PartnersDB._tr_get_all_fields_of_service)

        if len(names) < 1:
            return []
        else:
            return [FieldOfService(name=fos) for fos in names]

    @staticmethod
    def _tr_add_field_of_service(tx, field_of_service: FieldOfService):
        result = tx.run("MERGE (:FieldOfService {name: $name})",
                        name=field_of_service.name)

        records = list(result)
        summary = result.consume()
        return records, summary

    def add_field_of_service(self, field_of_service: FieldOfService):
        records, summary = self.session.execute_write(PartnersDB._tr_add_field_of_service,
                                                      field_of_service=field_of_service)
        if summary.counters.nodes_created < 1:
            return False
        else:
            return True

    @staticmethod
    def _tr_change_field_of_service(tx, old_name, new_data: FieldOfService):
        result = tx.run("""MATCH (w:FieldOfService{name: $area})
        SET w.name = $name""",
                        area=old_name,
                        name=new_data.name)
        records = list(result)
        summary = result.consume()
        return records, summary

    def change_field_of_service(self, old_name, new_data: FieldOfService):
        records, summary = self.session.execute_write(PartnersDB._tr_change_field_of_service,
                                                      old_name=old_name, new_data=new_data)
        if summary.counters.properties_set < 1:
            return "Field of service not found"
        else:
            return "Field of service updated"

    @staticmethod
    def _tr_delete_field_of_service(tx, name):
        tx.run("""MATCH (p:Partner)-[r]-(f:FieldOfService{name: $name}) DELETE r""", name=name)
        result = tx.run("""MATCH (f:FieldOfService{name: $name}) DELETE f""", name=name)
        records = list(result)
        summary = result.consume()
        return records, summary

    def delete_field_of_service(self, name):
        records, summary = self.session.execute_write(PartnersDB._tr_delete_field_of_service, name=name)

        if summary.counters.nodes_created < 1:
            return "Field of service not found"
        else:
            return "Field of service deleted"

    @staticmethod
    def _tr_get_partner(tx, name):
        result = tx.run("""MATCH (p:Partner {name: $name}) RETURN p""", name=name)
        records = list(result)

        partner = None if len(records) == 0 else records[0].data()['p']
        summary = result.consume()
        return partner, summary

    def get_partner(self, name) -> Union[Partner, None]:
        partner_dict, summary = self.session.execute_write(PartnersDB._tr_get_partner, name=name)

        if partner_dict is None:
            return partner_dict
        else:
            return Partner(**partner_dict)

    @staticmethod
    def _tr_get_all_partners(tx):
        result = tx.run("""MATCH (p:Partner) RETURN p""")
        records = list(result)

        partners = []
        for piece in records:
            partners.append(piece.data()['p'])
        summary = result.consume()
        return partners, summary

    def get_all_partners(self) -> list[Partner]:
        dicts, summary = self.session.execute_write(PartnersDB._tr_get_all_partners)

        if len(dicts) < 1:
            return []
        else:
            return [Partner(**p) for p in dicts]

    @staticmethod
    def _tr_get_all_partners_by_fos(tx, fos):
        result = tx.run("""MATCH (p:Partner)-[r]-(f:FieldOfService{name: $fos}) RETURN p""", fos=fos)
        records = list(result)

        partners = []
        for piece in records:
            partners.append(piece.data()['p'])
        summary = result.consume()
        return partners, summary

    def get_all_partners_by_fos(self, fos: str) -> list[Partner]:
        dicts, summary = self.session.execute_write(PartnersDB._tr_get_all_partners_by_fos, fos=fos)

        if len(dicts) < 1:
            return []
        else:
            return [Partner(**p) for p in dicts]

    @staticmethod
    def _tr_add_partner(tx, partner: Partner):
        try:
            result = tx.run(
                "MERGE (:Partner {name: $name, contact: $contact, audience_size: $auditory_qty, coef: $coef, trust_level: $trust_level})",
                name=partner.name,
                contact=partner.contact,
                auditory_qty=partner.audience_size,
                coef=partner.coef,
                trust_level=partner.trust_level
            )
        except neo4j.exceptions.ConstraintError:
            return [], None

        records = list(result)
        summary = result.consume()
        return records, summary

    def add_partner(self, partner: Partner):
        try:
            records, summary = self.session.execute_write(PartnersDB._tr_add_partner, partner=partner)
        except neo4j.exceptions.TransactionError:
            return False

        if summary.counters.nodes_created < 1:
            return False
        else:
            return True

    @staticmethod
    def _tr_change_partner(tx, old_name, new_data: Partner):
        result = tx.run("""MATCH (p:Partner{name: $old_name})
        SET p.name = $name, p.contact = $contact, p.audience_size = $auditory_qty, p.coef = $coef, p.trust_level = $trust_level""",
                        old_name=old_name,
                        name=new_data.name,
                        contact=new_data.contact,
                        coef=new_data.coef,
                        auditory_qty=new_data.audience_size,
                        trust_level=new_data.trust_level)
        records = list(result)
        summary = result.consume()
        return records, summary

    def change_partner(self, old_name, new_data: Partner):
        records, summary = self.session.execute_write(PartnersDB._tr_change_partner,
                                                      old_name=old_name, new_data=new_data)
        if summary.counters.nodes_created < 1:
            return "Partner not found"
        else:
            return "Partner updated"

    @staticmethod
    def _tr_delete_partner(tx, name):
        tx.run("""MATCH (p:Partner{name: $name})-[r]-(f:FieldOfService) DELETE r""", name=name)
        result = tx.run("""MATCH (p:Partner{name: $name}) DELETE p""", name=name)
        records = list(result)
        summary = result.consume()
        return records, summary

    def delete_partner(self, name):
        records, summary = self.session.execute_write(PartnersDB._tr_delete_partner, name=name)
        if summary.counters.nodes_created < 1:
            return "Partner not found"
        else:
            return "Partner deleted"

    @staticmethod
    def _tr_clear(tx):
        tx.run("""match (a) -[r] -> () delete a, r""")
        tx.run("""match (a) delete a""")

    def clear(self) -> None:
        self.session.execute_write(PartnersDB._tr_clear)
