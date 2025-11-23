# US3508 - As a System Administrator, I want a backup strategy to be proposed, justified, and implemented that minimizes RPO and WRT, so that the system can be rapidly restored after a failure with minimal data loss.

To meet the requirements of this User Story, a complete backup strategy was designed and implemented only for the database used by the system. The goal of the strategy is to ensure minimal data loss (RPO) and rapid recovery (WRT) while maintaining a low performance impact on the production environment.

### Backup Strategy Overview (Suggestion)

The strategy is built around the following principles:

**Minimizing RPO:**
- Frequent backups
- Local + remote storage (3-2-1 backup rule)
- Clear retention policy

**Minimizing WRT:**
- Backups stored locally for fast restoration
- Plain .sql dumps for simple and fast recovery
- Automatable restore process requiring minimal manual work

**Ensuring Availability and Security:**
- Backups scheduled during low-activity periods
- Off-site copy protects against local machine failure

A combination of weekly full backups and daily incremental backups can be chosen to ensure optimal balance between RPO, storage space, and recovery time.

**Weekly Full Backup**
- When: Sunday at 20:00
- Why: European ports experience significantly less activity on Sundays, allowing the heaviest backup to run with minimal impact.

**Daily Incremental Backups**
- When: Every day at 02:00
- Why: Activity on the system is lowest at this time
- Effect on RPO: Ensures RPO ≈ 24h (often less)

**Off-site Backup Replication**
- Following the 3-2-1 rule where 1 copy stored locally, 1 copy stored in secondary local storage and 1 copy transferred remotely (via SSH/SCP)

As for RPO and WRT values we can assume: 

**Recovery Point Objective (RPO)**
- Maximum RPO: 24 hours
- Practical expected RPO: < 12 hours (due to early-morning incremental backup)

**Work Recovery Time (WRT)**
- With local backups and an automated restore script the estimated WRT is of about 3 to 10 minutes

### Implementation

`/usr/local/bin/backup_database.sh` file:
```bash
#!/bin/bash

# Database settings
DB_USER="root"
DB_PASS="VNOmFHTv4yy0"
DB_HOST="vsgate-s1.dei.isep.ipp.pt"
DB_PORT="10383"
DB_NAME="port_management_db"

# Backup directory
BACKUP_DIR="/var/backups/pm_db"
LOG_FILE="/var/log/pm_backup.log"

mkdir -p $BACKUP_DIR

TIMESTAMP=$(date +"%Y%m%d_%H%M")
FILE_NAME="backup_${DB_NAME}_${TIMESTAMP}.sql"
BACKUP_PATH="${BACKUP_DIR}/${FILE_NAME}"

echo "[$(date)] Starting backup..." >> $LOG_FILE

# Perform MySQL dump
mysqldump -h $DB_HOST -P $DB_PORT -u $DB_USER -p$DB_PASS $DB_NAME \
  > $BACKUP_PATH 2>> $LOG_FILE

# Verify success
if [ $? -eq 0 ]; then
    echo "[$(date)] Backup completed: $FILE_NAME" >> $LOG_FILE
else
    echo "[$(date)] ERROR during backup!" >> $LOG_FILE
    exit 1
fi
```

To restore the system to a previous backup, we can run the following script:

`/usr/local/bin/restore_database.sh` file:
```bash
#!/bin/bash

BACKUP_FILE="$1"

# Database settings
DB_USER="root"
DB_PASS="VNOmFHTv4yy0"
DB_HOST="vsgate-s1.dei.isep.ipp.pt"
DB_PORT="10383"
DB_NAME="port_management_db"

# Check if backup file exists
if [ ! -f "$BACKUP_FILE" ]; then
    echo "Backup file not found: $BACKUP_FILE"
    exit 1
fi

echo "Restoring database '$DB_NAME' from backup: $BACKUP_FILE"

# Perform restore
mysql -h "$DB_HOST" -P "$DB_PORT" -u "$DB_USER" -p"$DB_PASS" "$DB_NAME" < "$BACKUP_FILE"

# Check result
if [ $? -eq 0 ]; then
    echo "Database restored successfully."
else
    echo "ERROR: Database restore failed!"
    exit 1
fi
```

### Testing

First, we execute our backup command:

```bash
root@vs228:/usr/local/bin# ./backup_database.sh
```

Then, we can check the log file:

```bash
root@vs228:/var/log# cat pm_backup.log
[Sat Nov 22 11:33:21 PM WET 2025] Starting backup...
[Sat Nov 22 11:33:23 PM WET 2025] Backup completed: backup_port_management_db_20251122_2333.sql
```

We can also check the sql file (trimmed because it's too long):

```bash
root@vs228:/var/backups/pm_db# cat backup_port_management_db_20251122_2333.sql
/*M!999999\- enable the sandbox mode */
-- MariaDB dump 10.19  Distrib 10.11.14-MariaDB, for debian-linux-gnu (x86_64)
--
-- Host: vsgate-s1.dei.isep.ipp.pt    Database: port_management_db
-- ------------------------------------------------------
-- Server version       8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
...
```