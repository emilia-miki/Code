import couchdb
from internal.models import *


class DocumentDB:
    def __init__(self, address='http://admin:admin@localhost:56497/'):
        self.server = couchdb.Server(address)
        try:
            self.templatesDB = self.server['templates']
        except couchdb.ResourceNotFound:
            self.templatesDB = self.server.create('templates')

        try:
            self.htmlsDB = self.server['htmls']
        except couchdb.ResourceNotFound:
            self.htmlsDB = self.server.create('htmls')

        try:
            self.stylesDB = self.server['styles']
        except couchdb.ResourceNotFound:
            self.stylesDB = self.server.create('styles')

    def add_template(self, template: Template) -> str:
        template_dict = template._asdict()
        del template_dict['id']
        pk, _ = self.templatesDB.save(template_dict)
        return pk

    def add_html(self, html: Html) -> str:
        html_dict = html._asdict()
        del html_dict['id']
        pk, _ = self.htmlsDB.save(html_dict)
        return pk

    def add_style(self, style: Style) -> str:
        style_dict = style._asdict()
        del style_dict['id']
        pk, _ = self.stylesDB.save(style_dict)
        return pk

    def change_template(self, template: Template):
        template_doc = self.templatesDB.get(template.id)
        if template_doc is None:
            return "template not found"
        template_doc.update(template._asdict())
        self.templatesDB[template.id] = template_doc
        return "template updated"

    def change_html(self, html: Html):
        html_doc = self.htmlsDB.get(html.id)
        if html_doc is None:
            return "html not found"
        html_doc.update(html._asdict())
        self.htmlsDB[html.id] = html_doc
        return "html updated"

    def change_style(self, style: Style):
        style_doc = self.stylesDB.get(style.id)
        if style_doc is None:
            return "style not found"
        style_doc.update(style._asdict())
        self.stylesDB[style.id] = style_doc
        return "style updated"

    def delete_template(self, pk: str):
        doc = self.templatesDB.get(pk)
        if doc is None:
            return "template not found"
        self.templatesDB.delete(doc)
        return "template deleted"

    def delete_html(self, pk: str):
        for docid in self.templatesDB.view('_all_docs'):
            i = docid['id']
            if self.templatesDB[i]['html_id'] == pk:
                return "html can't be deleted"

        doc = self.htmlsDB.get(pk)
        if doc is None:
            return "html not found"
        self.htmlsDB.delete(doc)
        return "html deleted"

    def delete_style(self, pk: str):
        for docid in self.templatesDB.view('_all_docs'):
            i = docid['id']
            for style_id in self.templatesDB[i]['style_ids']:
                if style_id == pk:
                    return "style can't be deleted"

        doc = self.stylesDB.get(pk)
        if doc is None:
            return "style not found"
        self.stylesDB.delete(doc)
        return "style deleted"

    def get_template_by_id(self, pk: str) -> Template:
        template_doc = self.templatesDB.get(pk)
        template_dict = dict(template_doc.items())
        del template_dict['_rev']
        template_dict['id'] = template_dict['_id']
        del template_dict['_id']
        return Template(**template_dict)

    def get_html_by_id(self, pk: str) -> Html:
        html_doc = self.htmlsDB.get(pk)
        html_dict = dict(html_doc.items())
        del html_dict['_rev']
        html_dict['id'] = html_dict['_id']
        del html_dict['_id']
        return Html(**html_dict)

    def get_style_by_id(self, pk: str) -> Style:
        style_doc = self.stylesDB.get(pk)
        style_dict = dict(style_doc.items())
        del style_dict['_rev']
        style_dict['id'] = style_dict['_id']
        del style_dict['_id']
        return Style(**style_dict)

    @staticmethod
    def get_all_formats() -> list[str]:
        dictionary = dict(AdFormat.__dict__)
        keys = list(dictionary.keys())
        for key in keys:
            if key.startswith('_'):
                del dictionary[key]

        return list(dictionary.keys())

    def get_all_templates(self) -> list[Template]:
        templates = []
        for docid in self.templatesDB.view('_all_docs'):
            i = docid['id']
            template_doc = self.templatesDB[i]
            template_dict = dict(template_doc.items())
            del template_dict['_rev']
            template_dict['id'] = template_dict['_id']
            del template_dict['_id']
            nt = Template(**template_dict)
            templates.append(nt)

        return templates

    def get_all_templates_by_format(self, template_format: AdFormat) -> list[Template]:
        templates = []
        for docid in self.templatesDB.view('_all_docs'):
            i = docid['id']
            template_doc = self.templatesDB[i]
            if template_doc['format'] == template_format:
                template_dict = dict(template_doc.items())
                del template_dict['_rev']
                template_dict['id'] = template_dict['_id']
                del template_dict['_id']
                templates.append(Template(**template_dict))

        return templates

    def get_all_htmls(self) -> list[Html]:
        htmls = []
        for docid in self.htmlsDB.view('_all_docs'):
            i = docid['id']
            html_doc = self.htmlsDB[i]
            html_dict = dict(html_doc.items())
            del html_dict['_rev']
            html_dict['id'] = html_dict['_id']
            del html_dict['_id']
            htmls.append(Html(**html_dict))

        return htmls

    def get_all_styles(self) -> list[Style]:
        styles = []
        for docid in self.stylesDB.view('_all_docs'):
            i = docid['id']
            style_doc = self.stylesDB[i]
            style_dict = dict(style_doc.items())
            del style_dict['_rev']
            style_dict['id'] = style_dict['_id']
            del style_dict['_id']
            styles.append(Style(**style_dict))

        return styles
