# Grafana Deployment Guide for vs228.dei.isep.ipp.pt

## Server Information
- **Host:** vs228.dei.isep.ipp.pt (10.9.20.228)
- **Project Path:** `/root/LEI-SEM5-PI-2025-26-3DJ-01`
- **Backend API:** Port 2226 (Internal) → https://vs-gate.dei.isep.ipp.pt:10228 (External)
- **Frontend:** Port 2229 (Internal) → https://vs-gate.dei.isep.ipp.pt:40228 (External)

## Prerequisites
SSH into your server:
```bash
ssh root@vs228.dei.isep.ipp.pt
# or via gateway
ssh -p 10228 root@vsgate-ssh.dei.isep.ipp.pt
```

## Installation Steps

### 1. Install Prometheus

```bash
# Download Prometheus
cd /opt
wget https://github.com/prometheus/prometheus/releases/download/v2.45.0/prometheus-2.45.0.linux-amd64.tar.gz
tar xvfz prometheus-2.45.0.linux-amd64.tar.gz
mv prometheus-2.45.0.linux-amd64 prometheus
rm prometheus-2.45.0.linux-amd64.tar.gz

# Create user
useradd --no-create-home --shell /bin/false prometheus

# Setup directories
mkdir -p /etc/prometheus
mkdir -p /var/lib/prometheus
chown -R prometheus:prometheus /var/lib/prometheus

# Copy config from your project
cp /root/LEI-SEM5-PI-2025-26-3DJ-01/prometheus.yml /etc/prometheus/
chown prometheus:prometheus /etc/prometheus/prometheus.yml

# Create systemd service
cat > /etc/systemd/system/prometheus.service <<EOF
[Unit]
Description=Prometheus
Wants=network-online.target
After=network-online.target

[Service]
User=prometheus
Group=prometheus
Type=simple
ExecStart=/opt/prometheus/prometheus \
    --config.file=/etc/prometheus/prometheus.yml \
    --storage.tsdb.path=/var/lib/prometheus/ \
    --web.listen-address=0.0.0.0:9090

[Install]
WantedBy=multi-user.target
EOF

# Start Prometheus
systemctl daemon-reload
systemctl start prometheus
systemctl enable prometheus
systemctl status prometheus
```

**Access Prometheus:** http://10.9.20.228:9090 (internal network only)

### 2. Install Loki

```bash
# Download Loki
cd /opt
wget https://github.com/grafana/loki/releases/download/v2.9.3/loki-linux-amd64.zip
apt-get install -y unzip
unzip loki-linux-amd64.zip
chmod +x loki-linux-amd64
rm loki-linux-amd64.zip

# Create user
useradd --no-create-home --shell /bin/false loki

# Setup directories
mkdir -p /etc/loki
mkdir -p /var/lib/loki
chown -R loki:loki /var/lib/loki

# Create Loki config
cat > /etc/loki/loki-config.yaml <<EOF
auth_enabled: false

server:
  http_listen_port: 3100
  grpc_listen_port: 9096

common:
  path_prefix: /var/lib/loki
  storage:
    filesystem:
      chunks_directory: /var/lib/loki/chunks
      rules_directory: /var/lib/loki/rules
  replication_factor: 1
  ring:
    kvstore:
      store: inmemory

schema_config:
  configs:
    - from: 2020-10-24
      store: boltdb-shipper
      object_store: filesystem
      schema: v11
      index:
        prefix: index_
        period: 24h

limits_config:
  reject_old_samples: true
  reject_old_samples_max_age: 168h
  ingestion_rate_mb: 10
  ingestion_burst_size_mb: 20

chunk_store_config:
  max_look_back_period: 0s

table_manager:
  retention_deletes_enabled: false
  retention_period: 0s
EOF

chown loki:loki /etc/loki/loki-config.yaml

# Create systemd service
cat > /etc/systemd/system/loki.service <<EOF
[Unit]
Description=Loki
After=network.target

[Service]
User=loki
Group=loki
Type=simple
ExecStart=/opt/loki-linux-amd64 -config.file=/etc/loki/loki-config.yaml
Restart=on-failure

[Install]
WantedBy=multi-user.target
EOF

# Start Loki
systemctl daemon-reload
systemctl start loki
systemctl enable loki
systemctl status loki
```

### 3. Install Promtail

```bash
# Download Promtail
cd /opt
wget https://github.com/grafana/loki/releases/download/v2.9.3/promtail-linux-amd64.zip
unzip promtail-linux-amd64.zip
chmod +x promtail-linux-amd64
rm promtail-linux-amd64.zip

# Create user
useradd --no-create-home --shell /bin/false promtail

# Setup directories
mkdir -p /etc/promtail
mkdir -p /var/lib/promtail
chown -R promtail:promtail /var/lib/promtail

# Copy config from your project
cp /root/LEI-SEM5-PI-2025-26-3DJ-01/promtail-config.yml /etc/promtail/
chown promtail:promtail /etc/promtail/promtail-config.yml

# Give promtail read access to log files
usermod -a -G root promtail
chmod 755 /root
chmod 755 /root/LEI-SEM5-PI-2025-26-3DJ-01
chmod 755 /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd
chmod 755 /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd/Api
chmod 755 /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd/Api/Logs
chmod 644 /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd/Api/Logs/*.log

# Create systemd service
cat > /etc/systemd/system/promtail.service <<EOF
[Unit]
Description=Promtail
After=network.target

[Service]
User=promtail
Group=promtail
Type=simple
ExecStart=/opt/promtail-linux-amd64 -config.file=/etc/promtail/promtail-config.yml
Restart=on-failure

[Install]
WantedBy=multi-user.target
EOF

# Start Promtail
systemctl daemon-reload
systemctl start promtail
systemctl enable promtail
systemctl status promtail
```

### 4. Install Grafana

```bash
# Add Grafana repository
apt-get install -y apt-transport-https software-properties-common wget
mkdir -p /etc/apt/keyrings/
wget -q -O - https://apt.grafana.com/gpg.key | gpg --dearmor | tee /etc/apt/keyrings/grafana.gpg > /dev/null
echo "deb [signed-by=/etc/apt/keyrings/grafana.gpg] https://apt.grafana.com stable main" | tee /etc/apt/sources.list.d/grafana.list

# Install Grafana
apt-get update
apt-get install -y grafana

# Configure Grafana to listen on a specific port (e.g., 3000)
# Edit /etc/grafana/grafana.ini if needed

# Start Grafana
systemctl daemon-reload
systemctl start grafana-server
systemctl enable grafana-server
systemctl status grafana-server
```

**Access Grafana:** http://10.9.20.228:3000 (internal network only)

## Configure Grafana

### 1. Access Grafana Web UI
- URL: http://10.9.20.228:3000
- Default credentials: **admin / admin**
- You'll be prompted to change the password

### 2. Add Prometheus Data Source
1. Go to **Configuration → Data Sources**
2. Click **Add data source**
3. Select **Prometheus**
4. Settings:
   - **Name:** Prometheus
   - **URL:** `http://localhost:9090`
   - **Access:** Server (default)
5. Click **Save & Test**

### 3. Add Loki Data Source
1. Go to **Configuration → Data Sources**
2. Click **Add data source**
3. Select **Loki**
4. Settings:
   - **Name:** Loki
   - **URL:** `http://localhost:3100`
   - **Access:** Server (default)
5. Click **Save & Test**

## Verify Everything is Working

### Check Services Status
```bash
systemctl status prometheus
systemctl status loki
systemctl status promtail
systemctl status grafana-server
```

### Check Logs
```bash
# Prometheus logs
journalctl -u prometheus -f

# Loki logs
journalctl -u loki -f

# Promtail logs
journalctl -u promtail -f

# Grafana logs
journalctl -u grafana-server -f
```

### Test Endpoints

```bash
# Test Prometheus is scraping your API
curl http://localhost:9090/api/v1/targets

# Test your API metrics endpoint
curl http://localhost:2226/metrics

# Test Loki is receiving logs
curl -G -s "http://localhost:3100/loki/api/v1/query" --data-urlencode 'query={job="aspnet-api"}' | jq

# Check Promtail is running
curl http://localhost:9080/ready
```

## Create Your First Dashboard

### Import ASP.NET Core Dashboard
1. In Grafana, go to **Dashboards → Import**
2. Enter dashboard ID: **10915**
3. Select **Prometheus** as data source
4. Click **Import**

### Create Log Panel
1. Create a new dashboard
2. Add a panel
3. Select **Loki** as data source
4. Query: `{job="aspnet-api"}`
5. Set visualization to **Logs**

## Sample Queries

### Prometheus Metrics

**Request Rate:**
```promql
rate(http_requests_received_total[5m])
```

**Response Time (95th percentile):**
```promql
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))
```

**Error Rate:**
```promql
sum(rate(http_requests_received_total{code=~"5.."}[5m]))
```

### Loki Logs

**All API logs:**
```logql
{job="aspnet-api"}
```

**Error logs only:**
```logql
{job="aspnet-api"} |= "ERR"
```

**Request types (1000-1005):**
```logql
{job="aspnet-api"} | regexp `Id: (?P<request_id>100[0-5])`
```

**Created actions (1000):**
```logql
{job="aspnet-api"} |~ "Id: 1000"
```

## Troubleshooting

### Promtail can't read log files
```bash
# Check permissions
ls -la /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd/Api/Logs/

# Check Promtail user can access
sudo -u promtail ls /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd/Api/Logs/

# If access denied, adjust permissions:
chmod 755 /root
chmod -R 755 /root/LEI-SEM5-PI-2025-26-3DJ-01
chmod 644 /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd/Api/Logs/*.log
```

### Prometheus can't scrape metrics
```bash
# Test from server
curl http://localhost:2226/metrics

# Check if API is running
systemctl status your-api-service  # Replace with your API service name
netstat -tlnp | grep 2226
```

### No logs in Loki
```bash
# Check Promtail logs
journalctl -u promtail -n 50

# Check if log files exist
ls -la /root/LEI-SEM5-PI-2025-26-3DJ-01/BackEnd/Api/Logs/

# Test Promtail config
/opt/promtail-linux-amd64 -config.file=/etc/promtail/promtail-config.yml -dry-run
```

## Public Access (Optional)

If you want to expose Grafana externally, you'll need to:

1. **Use a reverse proxy (nginx)** on one of your available ports
2. **Configure SSL/TLS** for secure access
3. **Set up authentication** (Grafana supports LDAP, OAuth, etc.)

Example nginx config for port 2224 → Grafana:
```nginx
server {
    listen 2224;
    server_name vs228.dei.isep.ipp.pt;

    location / {
        proxy_pass http://localhost:3000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

Then access via: http://vsgate-http.dei.isep.ipp.pt:10228

## Port Summary

| Service | Internal Port | Purpose |
|---------|---------------|---------|
| API Backend | 2226 | ASP.NET Core API (Public: vs-gate.dei.isep.ipp.pt:10228) |
| Frontend | 2229 | Vue.js App (Public: vs-gate.dei.isep.ipp.pt:40228) |
| Prometheus | 9090 | Metrics collection (Internal only) |
| Loki | 3100 | Log aggregation (Internal only) |
| Promtail | 9080 | Log shipping (Internal only) |
| Grafana | 3000 | Dashboard UI (Internal only, expose via nginx if needed) |
