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


@app.route("/api/molecular/<name>/<mole>", methods=["GET"])
def get_complexe_molecule(name, mole):
    sub_molecule_dir = os.path.join(MOL_DIR, name)
    molecule_file = f"{os.path.join(sub_molecule_dir, mole)}.cjson"

    if os.path.exists(molecule_file):
        with open(molecule_file) as f:
            data = json.load(f)
            return jsonify(get_information(data))


@app.route("/api/molecular/<mole>", methods=["GET"])
def get_molecule(mole):
    molecule_file = f"{os.path.join(MOL_DIR, mole)}.cjson"

    if os.path.exists(molecule_file):
        with open(molecule_file) as f:
            data = json.load(f)
            return jsonify(get_information(data))

@app.route("/api/atom/jmol/", methods=["GET"])
def get_jmol_list():
    molecule_file = f"{os.path.join(APPLICATION_PATH, 'atom')}.json"
    if os.path.exists(molecule_file):
        with open(molecule_file) as f:
            data = json.load(f)
            return jsonify((data))
        

if __name__ == "__main__":
    app.run(debug=True)
