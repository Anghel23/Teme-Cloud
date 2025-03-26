import json
import uuid
from http.server import BaseHTTPRequestHandler, HTTPServer

database = {}

class SimpleRESTHandler(BaseHTTPRequestHandler):
    def _send_response(self, status, data=None):
        self.send_response(status)
        self.send_header('Content-Type', 'application/json')
        self.end_headers()
        if data is not None:
            self.wfile.write(json.dumps(data).encode('utf-8'))

    def do_GET(self):
        global database
        if self.path == "/vulnerabilities":
            self._send_response(200, list(database.values()))
        elif self.path.startswith("/vulnerabilities/"):
            vuln_id = self.path.split("/")[-1]
            if vuln_id in database:
                self._send_response(200, database[vuln_id])
            else:
                self._send_response(404, {"error": "Vulnerability not found"})
        else:
            self._send_response(404, {"error": "Invalid endpoint"})

    def do_POST(self):
        global database
        content_length = int(self.headers['Content-Length'])
        post_data = json.loads(self.rfile.read(content_length))
        new_id = str(uuid.uuid4())
        database[new_id] = {
            "id": new_id,
            "name": post_data.get("name", ""),
            "code": post_data.get("code", ""),
            "severity": post_data.get("severity", ""),
            "description": post_data.get("description", "")
        }
        self._send_response(201, database[new_id])

    def do_PUT(self):
        global database
        if self.path.startswith("/vulnerabilities/"):
            vuln_id = self.path.split("/")[-1]
            if vuln_id in database:
                content_length = int(self.headers['Content-Length'])
                put_data = json.loads(self.rfile.read(content_length))
                database[vuln_id].update({
                    "name": put_data.get("name", database[vuln_id]["name"]),
                    "code": put_data.get("code", database[vuln_id]["code"]),
                    "severity": put_data.get("severity", database[vuln_id]["severity"]),
                    "description": put_data.get("description", database[vuln_id]["description"])
                })
                self._send_response(200, database[vuln_id])
            else:
                self._send_response(404, {"error": "Vulnerability not found"})
        else:
            self._send_response(404, {"error": "Invalid endpoint"})

    def do_DELETE(self):
        global database
        if self.path.startswith("/vulnerabilities/"):
            vuln_id = self.path.split("/")[-1]
            if vuln_id in database:
                del database[vuln_id]
                self._send_response(204)
            else:
                self._send_response(404, {"error": "Vulnerability not found"})
        else:
            self._send_response(404, {"error": "Invalid endpoint"})

if __name__ == "__main__":
    server_address = ('', 6969)
    httpd = HTTPServer(server_address, SimpleRESTHandler)
    print("Starting server on port 6969...")
    httpd.serve_forever()
