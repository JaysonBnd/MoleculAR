import base64
from email.mime import image
from typing import Any
from flask import Flask, jsonify
from flask_restful import Resource, Api
import os
import json

app = Flask(__name__)
api = Api(app)

APPLICATION_PATH = os.path.dirname(__file__)
MOL_DIR = os.path.join(APPLICATION_PATH, "mol")


def get_information(molecule_json: dict[str, Any]) -> dict[str, Any]:
    return {
        "coords": molecule_json["atoms"]["coords"]["3d"],
        "elements": molecule_json["atoms"]["elements"]["number"],
        "bonds": molecule_json["bonds"],
    }


@app.route("/api/molecule/<family>/<mole>", methods=["GET"])
def get_molecule_data(family, mole):
    sub_molecule_dir = os.path.join(MOL_DIR, family)
    molecule_file = f"{os.path.join(sub_molecule_dir, mole)}.cjson"

    if os.path.exists(molecule_file):
        with open(molecule_file) as f:
            data = json.load(f)
            return jsonify(get_information(data))


@app.route("/api/molecule/<family>/", methods=["GET"])
def get_family_data(family):
    molecule_dict: dict[str, Any] = {"molecules": []}
    dir_path = os.path.join(MOL_DIR, family)

    files_list = os.listdir(dir_path)
    for file in files_list:
        url_path = os.path.splitext(file)[0]
        file_name = url_path.title().replace("_", ",")

        if file_name not in molecule_dict["molecules"]:
            with open(f"{os.path.join(dir_path, url_path)}.png", "r+b") as image_file:
                encoded_image = base64.b64encode(image_file.read())

            molecule_dict["molecules"].append(
                {
                    "path": url_path,
                    "name": file_name,
                    "image": encoded_image.decode("utf-8"),
                }
            )

    return jsonify(molecule_dict)


@app.route("/api/molecule/", methods=["GET"])
def get_all_molecules():
    families_dict: dict[str, Any] = {"families": []}
    dirs_list = os.listdir(MOL_DIR)
    for dir in dirs_list:
        dir_path = os.path.join(MOL_DIR, dir)

        if os.path.isdir(dir_path):
            family_name = dir.capitalize()

            families_dict["families"].append({"path": dir, "name": family_name})

    return jsonify(families_dict)


@app.route("/api/atom/jmol/color", methods=["GET"])
def get_jmol_color_list():
    jmol_color_file = f"{os.path.join(APPLICATION_PATH, 'atom_color')}.json"
    if os.path.exists(jmol_color_file):
        with open(jmol_color_file) as f:
            data = json.load(f)
            return jsonify((data))


@app.route("/api/atom/jmol/scale", methods=["GET"])
def get_jmol_scale_list():
    jmol_scale_file = f"{os.path.join(APPLICATION_PATH, 'atom_size')}.json"
    if os.path.exists(jmol_scale_file):
        with open(jmol_scale_file) as f:
            data = json.load(f)
            return jsonify((data))


@app.route("/api/atom", methods=["GET"])
def get_atom():
    atom_file = f"{os.path.join(APPLICATION_PATH, 'atom')}.json"
    if os.path.exists(atom_file):
        with open(atom_file) as f:
            data = json.load(f)
            return jsonify((data))


if __name__ == "__main__":
    app.run(host="0.0.0.0", debug=True)