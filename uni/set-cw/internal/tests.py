from . import couchdb
from django.test import TestCase
from .models import *
from . import neo4j


class CouchTests(TestCase):
    def setUp(self):
        db = couchdb.DocumentDB()
        db.server.delete('templates')
        db.server.delete('htmls')
        db.server.delete('styles')

    def test_get_all_templates(self):
        db = couchdb.DocumentDB()
        t1 = Template("", "", list(), AdFormat.Billboard, 10)
        t2 = Template("", "", list(), AdFormat.MagazineTextWithMedia, 20)
        t3 = Template("", "", list(), AdFormat.MagazinePhoto, 30)
        db.templatesDB.save(t1._asdict())
        db.templatesDB.save(t2._asdict())
        db.templatesDB.save(t3._asdict())

        templates = db.get_all_templates()

        self.assertEqual(len(templates), 3, "should fetch 3 templates")

    def test_get_all_htmls(self):
        db = couchdb.DocumentDB()
        h1 = Html("", "content1")
        h2 = Html("", "content2")
        h3 = Html("", "content3")
        db.htmlsDB.save(h1._asdict())
        db.htmlsDB.save(h2._asdict())
        db.htmlsDB.save(h3._asdict())

        htmls = db.get_all_htmls()

        self.assertEqual(len(htmls), 3, "should fetch 3 htmls")

    def test_get_all_templates_by_format(self):
        db = couchdb.DocumentDB()
        t1 = Template("", "", list(), AdFormat.Billboard, 10)
        t2 = Template("", "", list(), AdFormat.Billboard, 20)
        t3 = Template("", "", list(), AdFormat.MagazinePhoto, 30)
        db.templatesDB.save(t1._asdict())
        db.templatesDB.save(t2._asdict())
        db.templatesDB.save(t3._asdict())

        templates = db.get_all_templates_by_format(AdFormat.Billboard)

        self.assertEqual(len(templates), 2, "should get 2 templates")
        self.assertEqual(templates[0].format, AdFormat.Billboard,
                         "fetched templates should all have Billboard format")
        self.assertEqual(templates[1].format, AdFormat.Billboard,
                         "fetched templates should all have Billboard format")

    def test_get_all_styles(self):
        db = couchdb.DocumentDB()
        s1 = Style("", "content1")
        s2 = Style("", "content2")
        s3 = Style("", "content3")
        db.stylesDB.save(s1._asdict())
        db.stylesDB.save(s2._asdict())
        db.stylesDB.save(s3._asdict())

        styles = db.get_all_styles()

        self.assertEqual(len(styles), 3, "should fetch 3 styles")

    def test_add_template(self):
        db = couchdb.DocumentDB()
        template = Template("", "", list(), AdFormat.Billboard, 10)
        db.add_template(template)
        templates = db.get_all_templates()

        self.assertEqual(len(templates), 1, "should contain one template")
        self.assertNotEqual(template.id, templates[0].id, "template should get an assigned id")
        self.assertEqual(template.html_id, templates[0].html_id, "html id should be equal")
        self.assertEqual(template.style_ids, templates[0].style_ids, "style ids should be equal")
        self.assertEqual(template.format, templates[0].format, "formats should be equal")
        self.assertEqual(template.price, templates[0].price, "prices should be equal")

    def test_add_html(self):
        db = couchdb.DocumentDB()
        html = Html("", "content")
        db.add_html(html)
        htmls = db.get_all_htmls()

        self.assertEqual(len(htmls), 1, "should contain one html")
        self.assertNotEqual(html.id, htmls[0].id, "html should get an assigned id")
        self.assertEqual(html.content, htmls[0].content, "html contents should be equal")

    def test_add_style(self):
        db = couchdb.DocumentDB()
        style = Style("", "content")
        db.add_style(style)
        styles = db.get_all_styles()

        self.assertEqual(len(styles), 1, "should contain one style")
        self.assertNotEqual(style.id, styles[0].id, "html should get an assigned id")
        self.assertEqual(style.content, styles[0].content, "html contents should be equal")

    def test_change_template(self):
        db = couchdb.DocumentDB()
        original_template = Template("", "", list(), AdFormat.Billboard, 10)
        db.add_template(original_template)
        templates = db.get_all_templates()
        old_id = templates[0].id
        new_template = Template(templates[0].id, 2, templates[0].style_ids,
                                templates[0].format, templates[0].price)
        db.change_template(new_template)
        templates = db.get_all_templates()

        self.assertEqual(len(templates), 1, "should contain one template")
        self.assertEqual(old_id, templates[0].id, "the id should not change")
        self.assertEqual(templates[0].html_id, 2, "html_id should get updated")
        self.assertEqual(templates[0].style_ids, original_template.style_ids, "style_ids should not change")
        self.assertEqual(templates[0].format, original_template.format, "format should not change")
        self.assertEqual(templates[0].price, original_template.price, "price should not change")

    def test_change_html(self):
        db = couchdb.DocumentDB()
        original_html = Html("", "content")
        db.add_html(original_html)
        htmls = db.get_all_htmls()
        old_id = htmls[0].id
        new_html = Html(htmls[0].id, "new content")
        db.change_html(new_html)
        htmls = db.get_all_htmls()

        self.assertEqual(len(htmls), 1, "should contain one html")
        self.assertEqual(old_id, htmls[0].id, "the id should not change")
        self.assertEqual(htmls[0].content, "new content", "content should get updated")

    def test_change_style(self):
        db = couchdb.DocumentDB()
        original_style = Style("", "content")
        db.add_style(original_style)
        styles = db.get_all_styles()
        old_id = styles[0].id
        new_style = Style(styles[0].id, "new content")
        db.change_style(new_style)
        styles = db.get_all_styles()

        self.assertEqual(len(styles), 1, "should contain one html")
        self.assertEqual(old_id, styles[0].id, "the id should not change")
        self.assertEqual(styles[0].content, "new content", "content should get updated")

    def test_delete_template(self):
        db = couchdb.DocumentDB()
        db.add_template(Template("", "", list(), AdFormat.Billboard, 10))
        templates = db.get_all_templates()
        db.delete_template(templates[0].id)
        templates = db.get_all_templates()

        self.assertEqual(len(templates), 0, "should contain no templates")

    def test_delete_html(self):
        db = couchdb.DocumentDB()
        db.add_html(Html("", "content"))
        htmls = db.get_all_htmls()
        db.delete_html(htmls[0].id)
        htmls = db.get_all_htmls()

        self.assertEqual(len(htmls), 0, "should contain no htmls")

    def test_delete_style(self):
        db = couchdb.DocumentDB()
        db.add_style(Style("", "content"))
        styles = db.get_all_styles()
        db.delete_style(styles[0].id)
        styles = db.get_all_styles()

        self.assertEqual(len(styles), 0, "should contain no styles")

    def test_get_template_by_id(self):
        db = couchdb.DocumentDB()
        db.add_template(Template("", 0, list(), AdFormat.Billboard, 10))
        templates = db.get_all_templates()
        template = db.get_template_by_id(templates[0].id)

        self.assertIsNotNone(template, "should return a template")
        self.assertEqual(templates[0].id, template.id, "returned the wrong template")

    def test_get_html_by_id(self):
        db = couchdb.DocumentDB()
        db.add_html(Html("", "content"))
        htmls = db.get_all_htmls()
        html = db.get_html_by_id(htmls[0].id)

        self.assertIsNotNone(html, "should return a html")
        self.assertEqual(htmls[0].id, html.id, "returned the wrong html")

    def test_get_style_by_id(self):
        db = couchdb.DocumentDB()
        db.add_style(Style("", "content"))
        styles = db.get_all_styles()
        style = db.get_style_by_id(styles[0].id)

        self.assertIsNotNone(style, "should return a style")
        self.assertEqual(styles[0].id, style.id, "returned the wrong style")

    def test_get_formats(self):
        db = couchdb.DocumentDB()
        formats = ['MagazineText', 'MagazineTextWithMedia', 'MagazinePhoto',
                   'Billboard', 'WebVideo', 'WebImage']
        test_formats = db.get_all_formats()

        for f in formats:
            self.assertIn(f, test_formats, "the returned array is different from AdFormat options")
        for f in test_formats:
            self.assertIn(f, formats, "the returned array is different from AdFormat options")

    def test_delete_html_referenced_by_template(self):
        db = couchdb.DocumentDB()
        pk1 = db.add_html(Html("", "content1"))
        pk2 = db.add_html(Html("", "content2"))
        db.add_template(Template("", pk1, [], AdFormat.Billboard, 10.0))

        status1 = db.delete_html(pk1)
        status2 = db.delete_html(pk2)

        self.assertEqual(status1, "html can't be deleted",
                         "html should not be deleted, because it is referenced in a template")
        self.assertEqual(status2, "html deleted",
                         "html should be deleted, because it is not referenced in any template")

    def test_delete_style_referenced_by_template(self):
        db = couchdb.DocumentDB()
        pk1 = db.add_style(Style("", "content1"))
        pk2 = db.add_style(Style("", "content2"))
        db.add_template(Template("", "", [pk1], AdFormat.Billboard, 10.0))

        status1 = db.delete_style(pk1)
        status2 = db.delete_style(pk2)

        self.assertEqual(status1, "style can't be deleted",
                         "style should not be deleted, because it is referenced in a template")
        self.assertEqual(status2, "style deleted",
                         "style should be deleted, because it is not referenced in any template")


class Neo4jTests(TestCase):
    def setUp(self):
        db = neo4j.PartnersDB()
        db.clear()

    def test_get_all_partners(self):
        db = neo4j.PartnersDB()
        db.add_partner(Partner(name="McDonalds", contact="mc", audience_size=34, coef=5.2, trust_level=4))
        db.add_partner(Partner(name="Burger King", contact="burgir", audience_size=10, coef=2.3, trust_level=2))
        partners = db.get_all_partners()

        self.assertEqual(len(partners), 2, "Should contain 2 partners")

    def test_get_all_partners_by_fos(self):
        db = neo4j.PartnersDB()

        db.add_partner(Partner(name="McDonalds", contact="mc", audience_size=34, coef=5.2, trust_level=4))
        db.add_partner(Partner(name="Burger King", contact="burgir", audience_size=10, coef=2.3, trust_level=2))
        db.add_partner(Partner(name="KFC", contact="kfc", audience_size=10, coef=2.3, trust_level=2))

        db.add_partner(Partner(name="Microsoft", contact="", audience_size=0, coef=0.0, trust_level=2))
        db.add_partner(Partner(name="Apple", contact="", audience_size=0, coef=0.0, trust_level=2))

        db.add_field_of_service(FieldOfService(name="IT"))
        db.add_field_of_service(FieldOfService(name="Fast Food"))

        db.add_partner_to_fos_conn("McDonalds", "Fast Food")
        db.add_partner_to_fos_conn("Burger King", "Fast Food")
        db.add_partner_to_fos_conn("KFC", "Fast Food")

        db.add_partner_to_fos_conn("Microsoft", "IT")
        db.add_partner_to_fos_conn("Apple", "IT")

        it = db.get_all_partners_by_fos("IT")
        ff = db.get_all_partners_by_fos("Fast Food")

        self.assertEqual(len(it), 2, "IT FOS should return 2 partners")
        self.assertEqual(len(ff), 3, "Fast Food FOS should return 2 partners")

    def test_get_partner(self):
        db = neo4j.PartnersDB()
        db.add_partner(Partner(name="McDonalds", contact="mc", audience_size=34, coef=5.2, trust_level=4))
        partner = db.get_partner("McDonalds")

        self.assertIsNotNone(partner)
        self.assertIsInstance(partner, Partner, "Should return a Partner instance")
        self.assertEqual(partner.name, "McDonalds", "Should return McDonanlds")

    def test_add_partner_duplicate_name(self):
        db = neo4j.PartnersDB()
        db.add_partner(Partner(name="McDonalds", contact="mc", audience_size=34, coef=5.2, trust_level=4))
        db.add_partner(Partner(name="McDonalds", contact="", audience_size=0, coef=5.2, trust_level=4))
        partners = db.get_all_partners()

        self.assertEqual(len(partners), 1, "Should add only one partner")

    def test_change_partner(self):
        db = neo4j.PartnersDB()
        db.add_partner(Partner(name="McDonalds", contact="mc", audience_size=34, coef=5.2, trust_level=4))
        db.change_partner("McDonalds", Partner(name="Burger King", contact="burgir",
                                               audience_size=34, coef=5.2, trust_level=4))
        partners = db.get_all_partners()

        self.assertEqual(len(partners), 1, "Number of partners should not change")
        self.assertEqual(partners[0].name, "Burger King", "Partner should get renamed")
        self.assertEqual(partners[0].contact, "burgir", "Contact should be changed")
        self.assertEqual(partners[0].audience_size, 34, "Audience size should remain the same")

    def test_delete_partner(self):
        db = neo4j.PartnersDB()
        db.add_partner(Partner(name="McDonalds", contact="mc", audience_size=34, coef=5.2, trust_level=4))
        db.delete_partner("McDonalds")
        partners = db.get_all_partners()

        self.assertEqual(len(partners), 0, "Should contain no partners")

    def test_get_all_fields_of_service(self):
        db = neo4j.PartnersDB()
        db.add_field_of_service(FieldOfService(name="Construction"))
        db.add_field_of_service(FieldOfService(name="Programming"))
        fields_of_serivce = db.get_all_fields_of_service()

        self.assertEqual(len(fields_of_serivce), 2, "Should contain 2 fields of service")

    def test_add_field_of_service_duplicate_name(self):
        db = neo4j.PartnersDB()
        db.add_field_of_service(FieldOfService(name="Construction"))
        db.add_field_of_service(FieldOfService(name="Construction"))
        fields_of_serivce = db.get_all_fields_of_service()

        self.assertEqual(len(fields_of_serivce), 1, "Should add only one field of service")

    def test_change_field_of_service(self):
        db = neo4j.PartnersDB()
        db.add_field_of_service(FieldOfService(name="Construction"))
        db.change_field_of_service("Construction", FieldOfService(name="Engineering"))
        fields_of_service = db.get_all_fields_of_service()

        self.assertEqual(len(fields_of_service), 1, "Number of fields of service should not change")
        self.assertEqual(fields_of_service[0].name, "Engineering", "Field of service should get renamed")

    def test_delete_field_of_service(self):
        db = neo4j.PartnersDB()
        db.add_field_of_service(FieldOfService(name="Construction"))
        db.delete_field_of_service("Construction")
        fields_of_service = db.get_all_fields_of_service()

        self.assertEqual(len(fields_of_service), 0, "Should contain no fields of service")
