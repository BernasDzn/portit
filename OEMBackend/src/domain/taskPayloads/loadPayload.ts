// The container to be loaded and the storage location where it should be placed/picked from
// No container is less specific but possible, it is what is generated as a plan automatically, 
// a load operation "of all containers available" and then on a vve the actual container IDs are assigned by hand
export default interface LoadPayload {
    containerId: string | null;
    storageLocation: string | null;
}