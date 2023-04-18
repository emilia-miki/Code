from rest_framework.decorators import api_view
from rest_framework.response import Response
from rest_framework.request import Request
from . import couchdb
from .models import *
from . import neo4j


@api_view(["GET", "PUT", "POST"])
def templates(request: Request):
    db = couchdb.DocumentDB()
    if request.method == "GET":
        if request.query_params.get('f') is not None:
            try:
                f = AdFormat(int(request.query_params['f']))
            except ValueError:
                return Response({'templates': []})
            result = db.get_all_templates_by_format(f)
            return Response({'templates': [t._asdict() for t in result]})
        else:
            result = db.get_all_templates()
            return Response({'templates': [t._asdict() for t in result]})
    elif request.method == "PUT":
        return Response({'status': db.change_template(Template(**request.data['template']))})
    elif request.method == "POST":
        return Response({'id': db.add_template(Template(**request.data['template']))})


@api_view(["GET", "DELETE"])
def template(request: Request, pk: str):
    db = couchdb.DocumentDB()
    if request.method == "GET":
        return Response({'template': db.get_template_by_id(pk)._asdict()})
    elif request.method == "DELETE":
        return Response({'status': db.delete_template(pk)})


@api_view(["GET", "PUT", "POST"])
def htmls(request: Request):
    db = couchdb.DocumentDB()
    if request.method == "GET":
        return Response({'htmls': [h._asdict() for h in db.get_all_htmls()]})
    elif request.method == "PUT":
        return Response({'status': db.change_html(Html(**request.data['html']))})
    elif request.method == "POST":
        return Response({'id': db.add_html(Html(**request.data['html']))})


@api_view(["GET", "DELETE"])
def html(request: Request, pk: str):
    db = couchdb.DocumentDB()
    if request.method == "GET":
        return Response({'html': db.get_html_by_id(pk)._asdict()})
    elif request.method == "DELETE":
        return Response({'status': db.delete_html(pk)})


@api_view(["GET", "PUT", "POST"])
def styles(request: Request):
    db = couchdb.DocumentDB()
    if request.method == "GET":
        return Response({'styles': [s._asdict() for s in db.get_all_styles()]})
    elif request.method == "PUT":
        return Response({'status': db.change_style(Style(**request.data['style']))})
    elif request.method == "POST":
        return Response({'id': db.add_style(Style(**request.data['style']))})


@api_view(["GET", "DELETE"])
def style(request: Request, pk: str):
    db = couchdb.DocumentDB()
    if request.method == "GET":
        return Response({'style': db.get_style_by_id(pk)._asdict()})
    elif request.method == "DELETE":
        return Response({'status': db.delete_style(pk)})


@api_view(["GET"])
def get_formats(request: Request):
    db = couchdb.DocumentDB()
    return Response({'formats': db.get_all_formats()})


@api_view(["GET", "PUT"])
def partners(request: Request):
    db = neo4j.PartnersDB()
    if request.method == "GET":
        if request.query_params.get("fos") is not None:
            return Response({"partners": [p._asdict() for p in db.get_all_partners_by_fos(
                                            request.query_params["field_of_service"])]})
        else:
            return Response({"partners": [p._asdict() for p in db.get_all_partners()]})
    elif request.method == "PUT":
        return Response({"status": db.change_partner(request.data["old_name"], request.data["new_data"])})


@api_view(["GET", "DELETE"])
def partner(request: Request, pk: str):
    db = neo4j.PartnersDB()
    if request.method == "GET":
        return Response({"partner": db.get_partner(pk)})
    elif request.method == "DELETE":
        return Response({"status": db.delete_partner(pk)})


@api_view(["GET", "PUT"])
def fields_of_service(request: Request):
    db = neo4j.PartnersDB()
    if request.method == "GET":
        return Response({"fields_of_service": [f._asdict() for f in db.get_all_fields_of_service()]})
    elif request.method == "PUT":
        return Response({"status": db.change_field_of_service(
                         request.data["old_name"], request.data["field_of_service"])})


@api_view(["DELETE"])
def field_of_service(request: Request, pk: str):
    db = neo4j.PartnersDB()
    return Response({"status": db.delete_field_of_service(pk)})


@api_view(["POST", "DELETE"])
def connections(request: Request):
    db = neo4j.PartnersDB()
    if request.method == "POST":
        return Response({"status": db.add_partner_to_fos_conn(
                                        request.data["partner"], request.data["field_of_service"])})
    elif request.method == "DELETE":
        return Response({"status": db.del_partner_to_fos_conn(
                                        request.data["partner"], request.data["field_of_service"])})
