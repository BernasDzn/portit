# US3503 - As a System Administrator, I want the list of allowed client endpoints (as defined in US 3.5.2) to be configurable by editing a simple text or configuration file, so that access control remains easy to maintain without redeployment.

Unlike the implementation in US3502, where allowed IP addresses were hardcoded into the firewall script, in this approach we use a simple external file to define the allowed clients. This method allows the IP addresses or ranges to be modified easily without altering the firewall script itself. For testing purposes, we intentionally included some clearly invalid entries in the file to verify that the script correctly detects and logs them.

### Allowed clients file

The `/etc/allowed_clients.txt` file contains the list of IP addresses and ranges that are permitted to connect through the firewall:
```sh
# In this file we can write the endpoints that we allow
# connection through the firewall.
#
# Can be a range of IPs or a single ip per line, for example:
#
# Endpoints with the wrong format will be logged on "journalctl -t ip-validator"
#
# 10.0.0.0/30
# 10.1.0.20
# etc...

# DEI's network ip range and vs-gate
10.8.0.0/16
10.9.0.0/16

# invalid ips to be logged as errors (test)
abcd
0.0.0.0.12013.2
```
This file supports both individual IP addresses and CIDR notation ranges. Lines beginning with # are treated as comments and ignored. Any entry that does not conform to the IPv4 or CIDR format is logged for review.

#### Firewall Script

The `/etc/start_firewall` dynamically reads the allowed clients file and configures the firewall accordingly. The main steps are:
1. Flush previous rules using /etc/stop_firewall.

2. Read the allowed clients list, ignoring blank lines and comments.

3. Validate each IP using a regular expression and ensure that each octet falls within the correct range (0–255). Invalid entries are logged to the system journal using the ip-validator tag.

4. Add rules to allow TCP traffic to specific service ports (2226, 2228, 2229) for each valid IP or range.

5. Allow SSH connections (port 22) and already established connections.

6. Set the default policy to drop all other incoming traffic.

7. Persist the rules using iptables-save.

```sh
#!/bin/bash

IPLIST="allowed_clients.txt"
ipv4_or_cidr='^([0-9]{1,3}\.){3}[0-9]{1,3}(/([0-9]|[12][0-9]|3[0-2]))?$'

# Remove all previous chain rules
./stop_firewall

# Load allowed IPs from file
while IFS= read -r line; do
    ip="$(echo "$line" | sed 's/^[ \t]*//;s/[ \t]*$//')"

    [[ -z "$ip" || "$ip" == \#* ]] && continue
	# Check if ip is valid, else log invalid ip to "ip-validator"
    [[ $ip =~ $ipv4_or_cidr ]] || { logger -t ip-validator "Invalid IP: $ip"; continue; }

    IFS='./' read -ra parts <<< "$ip"
    valid=true
    for oct in "${parts[@]:0:4}"; do
        [[ "$oct" =~ ^[0-9]+$ && $oct -le 255 ]] || valid=false
    done

    if [[ $valid == true ]]; then
        echo "Allowing $ip"
        iptables -A INPUT -p tcp --dport 2229 -s "$ip" -j ACCEPT
		iptables -A INPUT -p tcp --dport 2226 -s "$ip" -j ACCEPT
		iptables -A INPUT -p tcp --dport 2228 -s "$ip" -j ACCEPT
    else
        logger -t ip-validator "Invalid octet in: $ip"
    fi
done < "$IPLIST"

# Accept SSH and established conns
iptables -A INPUT -p tcp --dport 22 -j ACCEPT
iptables -A INPUT -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT

# Deny other accesses
iptables -P INPUT DROP

# Save iptable rules
iptables-save > /etc/iptables/rules.v4

echo "Firewall rules applied successfully."
```

#### Script Execution and Logging

When executed, the script outputs the following summary:

```sh
root@vs1014:/etc# ./start_firewall
Creating temporary allow rules for SSH to avoid lockout...
Flushing all iptables rules...
Setting default policies...
Firewall disabled safely.
Allowing 10.8.0.0/16
Allowing 10.9.0.0/16
Firewall rules applied successfully.
```

Invalid entries are logged to the system journal:

```sh
root@vs1014:~# journalctl -t ip-validator
Nov 18 23:58:32 vs1014 ip-validator[9373]: Invalid IP format: abcd
Nov 18 23:58:32 vs1014 ip-validator[9374]: Invalid IP format: 0.0.0.0.12013.2
```

#### Verification

After connecting to the ISEP VPN on our personal machine, we can verify that the IP is within our defined range:

```powershell
PS C:\Users\Rui> ipconfig
...
PPP adapter ISEP VPN:
   Connection-specific DNS Suffix  . : dei.isep.ipp.pt
   IPv4 Address. . . . . . . . . . . : 10.8.227.143
   Subnet Mask . . . . . . . . . . . : 255.255.255.255
   Default Gateway . . . . . . . . . : 0.0.0.0
...
```

To see the packets being received, we can connect to the ip multiple times in the browser and then inspect the firewall rules using:

```shell
root@vs1014:~# iptables -nvL
Chain INPUT (policy DROP 0 packets, 0 bytes)
 pkts bytes target     prot opt in     out     source               destination
  488  194K ACCEPT     6    --  *      *       10.8.0.0/16          0.0.0.0/0            tcp dpt:2229
    0     0 ACCEPT     6    --  *      *       10.8.0.0/16          0.0.0.0/0            tcp dpt:2226
    0     0 ACCEPT     6    --  *      *       10.8.0.0/16          0.0.0.0/0            tcp dpt:2228
    0     0 ACCEPT     6    --  *      *       10.9.0.0/16          0.0.0.0/0            tcp dpt:2229
    0     0 ACCEPT     6    --  *      *       10.9.0.0/16          0.0.0.0/0            tcp dpt:2226
    0     0 ACCEPT     6    --  *      *       10.9.0.0/16          0.0.0.0/0            tcp dpt:2228
   13   888 ACCEPT     6    --  *      *       0.0.0.0/0            0.0.0.0/0            tcp dpt:22
   33 10455 ACCEPT     0    --  *      *       0.0.0.0/0            0.0.0.0/0            ctstate RELATED,ESTABLISHED
```

As we can see, in the frontend rule (port 2229) shows that 488 packets were received and accepted from sources within the 10.8.0.0/16 range, confirming that traffic from the VPN is correctly permitted.