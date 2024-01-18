from flask import Flask, jsonify
from flask_restful import Resource, Api
import os
import json

app = Flask(__name__)
api = Api(app)

@app.route('/api/molecular/<name>/<mole>', methods=['GET'])
def get_user(name, mole):
    pwd = os.getcwd()
    dirs = os.listdir('./mol/')
    file = None
    for dir in dirs:
        if name == dir:
            files = os.listdir(pwd + '/mol/' + name)
            for file in files:
                if file.endswith(".cjson") and os.path.splitext(file)[0] == mole:
                    with open(pwd + '/mol/' + name + '/' + file) as f:
                        data = json.load(f)
                        return jsonify(data)
    

if __name__ == '__main__':
    app.run(debug=True)