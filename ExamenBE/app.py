from flask import Flask, request, jsonify, render_template
from flask_cors import CORS
from datetime import datetime
import sqlite3
import os

app = Flask(__name__)
CORS(app)

DB_FILE = 'locations.db'

def init_db():
    conn = sqlite3.connect(DB_FILE)
    c = conn.cursor()
    c.execute('''
        CREATE TABLE IF NOT EXISTS locations (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            device_id TEXT NOT NULL,
            latitude REAL NOT NULL,
            longitude REAL NOT NULL,
            accuracy REAL DEFAULT 0,
            timestamp TEXT NOT NULL,
            created_at TEXT DEFAULT CURRENT_TIMESTAMP
        )
    ''')
    c.execute('CREATE INDEX IF NOT EXISTS idx_device_id ON locations(device_id)')
    c.execute('CREATE INDEX IF NOT EXISTS idx_timestamp ON locations(timestamp)')
    conn.commit()
    conn.close()

init_db()

def get_db():
    conn = sqlite3.connect(DB_FILE)
    conn.row_factory = sqlite3.Row
    return conn

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/api/location', methods=['GET', 'POST'])
def receive_location():
    try:
        if request.method == 'GET':
            device_id = request.args.get('device_id')
            lat = float(request.args.get('lat', 0))
            lon = float(request.args.get('lon', 0))
            accuracy = float(request.args.get('accuracy', 0))
            timestamp = request.args.get('timestamp', datetime.now().isoformat())
        else:
            data = request.get_json()
            device_id = data.get('device_id')
            lat = float(data.get('lat', 0))
            lon = float(data.get('lon', 0))
            accuracy = float(data.get('accuracy', 0))
            timestamp = data.get('timestamp', datetime.now().isoformat())

        if not device_id:
            return jsonify({'error': 'device_id required'}), 400

        conn = get_db()
        c = conn.cursor()
        c.execute('''
            INSERT INTO locations (device_id, latitude, longitude, accuracy, timestamp)
            VALUES (?, ?, ?, ?, ?)
        ''', (device_id, lat, lon, accuracy, timestamp))
        conn.commit()
        conn.close()

        return jsonify({
            'status': 'ok',
            'received': {
                'device_id': device_id,
                'lat': lat,
                'lon': lon,
                'accuracy': accuracy,
                'timestamp': timestamp
            }
        })
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/api/devices')
def get_devices():
    conn = get_db()
    c = conn.cursor()
    c.execute('''
        SELECT device_id,
               latitude, longitude, accuracy, timestamp,
               COUNT(*) as total_records
        FROM locations
        GROUP BY device_id
        ORDER BY MAX(created_at) DESC
    ''')

    devices = []
    for row in c.fetchall():
        # Obtener ultima ubicacion
        c.execute('''
            SELECT latitude, longitude, accuracy, timestamp
            FROM locations
            WHERE device_id = ?
            ORDER BY timestamp DESC
            LIMIT 1
        ''', (row['device_id'],))
        latest = c.fetchone()

        devices.append({
            'device_id': row['device_id'],
            'current': {
                'lat': latest['latitude'],
                'lon': latest['longitude'],
                'accuracy': latest['accuracy'],
                'timestamp': latest['timestamp']
            },
            'history_count': row['total_records']
        })

    conn.close()
    return jsonify(devices)

@app.route('/api/device/<device_id>')
def get_device(device_id):
    conn = get_db()
    c = conn.cursor()

    # Ultima ubicacion
    c.execute('''
        SELECT latitude, longitude, accuracy, timestamp
        FROM locations
        WHERE device_id = ?
        ORDER BY timestamp DESC
        LIMIT 1
    ''', (device_id,))
    current = c.fetchone()

    if not current:
        conn.close()
        return jsonify({'error': 'Device not found'}), 404

    # Historial completo
    c.execute('''
        SELECT latitude as lat, longitude as lon, accuracy, timestamp
        FROM locations
        WHERE device_id = ?
        ORDER BY timestamp ASC
    ''', (device_id,))
    history = [dict(row) for row in c.fetchall()]

    conn.close()
    return jsonify({
        'current': {
            'lat': current['latitude'],
            'lon': current['longitude'],
            'accuracy': current['accuracy'],
            'timestamp': current['timestamp']
        },
        'history': history
    })

@app.route('/api/device/<device_id>/history')
def get_device_history(device_id):
    """
    Obtener historial con filtros opcionales de tiempo
    Query params:
    - start: fecha/hora inicio (ISO format)
    - end: fecha/hora fin (ISO format)
    """
    start = request.args.get('start')
    end = request.args.get('end')

    conn = get_db()
    c = conn.cursor()

    query = '''
        SELECT latitude as lat, longitude as lon, accuracy, timestamp
        FROM locations
        WHERE device_id = ?
    '''
    params = [device_id]

    if start:
        query += ' AND timestamp >= ?'
        params.append(start)

    if end:
        query += ' AND timestamp <= ?'
        params.append(end)

    query += ' ORDER BY timestamp ASC'

    c.execute(query, params)
    history = [dict(row) for row in c.fetchall()]

    conn.close()
    return jsonify(history)

@app.route('/api/device/<device_id>/stats')
def get_device_stats(device_id):
    """Obtener estadisticas del dispositivo"""
    conn = get_db()
    c = conn.cursor()

    c.execute('''
        SELECT
            COUNT(*) as total_records,
            MIN(timestamp) as first_record,
            MAX(timestamp) as last_record,
            AVG(accuracy) as avg_accuracy
        FROM locations
        WHERE device_id = ?
    ''', (device_id,))

    stats = c.fetchone()
    conn.close()

    if not stats or stats['total_records'] == 0:
        return jsonify({'error': 'Device not found'}), 404

    return jsonify({
        'total_records': stats['total_records'],
        'first_record': stats['first_record'],
        'last_record': stats['last_record'],
        'avg_accuracy': round(stats['avg_accuracy'], 2) if stats['avg_accuracy'] else 0
    })

@app.route('/api/clear/<device_id>', methods=['DELETE'])
def clear_device(device_id):
    conn = get_db()
    c = conn.cursor()
    c.execute('DELETE FROM locations WHERE device_id = ?', (device_id,))
    deleted = c.rowcount
    conn.commit()
    conn.close()

    if deleted > 0:
        return jsonify({'status': 'deleted', 'records': deleted})
    return jsonify({'error': 'Device not found'}), 404

@app.route('/api/clear-all', methods=['DELETE'])
def clear_all():
    conn = get_db()
    c = conn.cursor()
    c.execute('DELETE FROM locations')
    deleted = c.rowcount
    conn.commit()
    conn.close()
    return jsonify({'status': 'deleted', 'records': deleted})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
