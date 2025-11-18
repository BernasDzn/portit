# US3508 - As a System Administrator, I want a backup strategy to be proposed, justified, and implemented that minimizes RPO and WRT, so that the system can be rapidly restored after a failure with minimal data loss.

Backups are relevant to ensure that in the event of a critical failure or data loss, the system is easily recovered and operationability restored as fast as possible.

Privacy must be ensured, in the process of making the copies and where the copies are kept.

Mirroring on remote location:
- Facilitates disaster recovery
- Can be synchronous or asynchronous, the RPO and WRT are null if asynchronous and very close to it if synchronous

Possible strategies:
- Integral/Full: copies all data to a remote or on-site location, implies longer WRT
- Incremental: The first incremental backup requires a prior full copy, copies all changed data since the last integral backup
- Differential: Always needs a prior full copy and copies all changed data since the previous full copy

the best strategy to be used depends on two factors, the execution environment and the possiblity of keeping the system running during the copying.

A deeper research reveals the 3-2-1 rule is one of the best at dealing with the RPO. It states that organizations should keep three complete copies of their data, two of which are local and one copy stored off-site. For example, an organization should back up to a local on-premises backup storage system, copy that data to another on-premises backup storage system and then replicate that data to another location. (https://www.techtarget.com/searchdatabackup/definition/3-2-1-Backup-Strategy , https://www.comptia.org/en-eu/blog/5-it-disaster-recovery-measurements-to-know/)

European ports tend to have less activity during Sunday, therefore, making a Full copy is appropriate since it requires the most time. During the week, the system should make differential copies and save one of them on site and another in a remote.

Copying will always have some impact on server performance and
will also be made easier if no significant file system changes
are currently taking place.

In the context of our system and for testing purposes, the system will make a full backup every sunday at 20:00 and Integral copies every two days at 20:00. 

[perguntar ao stor se é para actually fazer backup]