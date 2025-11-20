# US3502 - As a System Administrator, I want access to the solution to be restricted to clients connected to the DEI internal network (wired or via VPN), so that the system remains secure and compliant with institutional access policies.

The first step in solving this problem consists of identifying the IP range of DEI’s internal network. After consulting the network configuration, we determined that the DEI subnet corresponds to 10.8.0.0/16.

For the purpose of this solution, we consider that each virtual machine of the system is accessible via the address format vsXXXX.dei.isep.ipp.pt:YYYY, where XXXX denotes the machine identifier and YYYY the listening port of a given service. Any reference to vs-gate is intentionally disregarded, as it is not relevant to this US and is impossible to configure ip connections, therefore 10.9.0.0/16 ips will be allowed.

To ensure that the system is only reachable when the user is connected to DEI's VPN, we configure a firewall on the host machine (running Linux) using iptables. Each module of the project listens on a distinct port: the frontend on 2229, the backend on 2226, and the Prolog service on 2228.

In order to conveniently enable or disable the firewall without risking a lockout of active SSH sessions, we created two independent scripts. The first script enables the firewall and applies all restrictions, whereas the second script disables it safely by ensuring that ongoing connections and SSH remain unaffected.

File `/etc/us1_start_firewall` :
```sh
#!/bin/bash

iptables -A INPUT -p tcp --dport 2229 -s 10.8.0.0/16 -j ACCEPT
iptables -A INPUT -p tcp --dport 2226 -s 10.8.0.0/16 -j ACCEPT
iptables -A INPUT -p tcp --dport 2228 -s 10.8.0.0/16 -j ACCEPT
iptables -A INPUT -s 10.9.0.0/16 -j ACCEPT
iptables -A INPUT -p tcp --dport 22 -j ACCEPT
iptables -A INPUT -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT

iptables -P INPUT DROP

echo "Firewall rules applied successfully."
```

File `/etc/stop_firewall` :
```sh
#!/bin/bash

iptables -I INPUT 1 -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT
iptables -I INPUT 2 -p tcp --dport 22 -j ACCEPT

echo "Flushing all iptables rules..."
iptables -F
iptables -X

echo "Setting default policies..."
iptables -P INPUT ACCEPT
iptables -P FORWARD ACCEPT
iptables -P OUTPUT ACCEPT

echo "Firewall disabled safely."
```

Explanation of the commands:

1. **iptables -A INPUT -p tcp --dport XXXX -s 10.8.0.0/16 -j ACCEPT**
Appends a rule to the INPUT chain allowing TCP connections to port XXXX, but only if the source IP belongs to the DEI VPN subnet 10.8.0.0/16. This ensures that the service is reachable exclusively through the VPN.

2. **iptables -A INPUT -s 10.9.0.0/16 -j ACCEPT**
Appends a rule to the INPUT chain allowing connections from the source IP belongs 10.9.0.0/16. This ensures that vs-gate connections are allowed and project is not compromissed.

4. **iptables -A INPUT -p tcp --dport 22 -j ACCEPT**
Ensures unrestricted SSH access (port 22) from any source. This rule is essential for maintaining access to the machine.

5. **iptables -A INPUT -m conntrack --ctstate ESTABLISHED,RELATED -j ACCEPT**
Allows packets that are part of already established or related connections. This rule preserves the continuity of ongoing connections, including SSH sessions, HTTP responses, and other valid traffic negotiated previously.

6. **iptables -P INPUT DROP**
Sets the default policy of the INPUT chain to DROP. Any incoming packet that does not match a previously defined ACCEPT rule is rejected. This effectively enforces the firewall restrictions by denying all unspecified traffic.
