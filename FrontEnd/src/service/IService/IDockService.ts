import { type Dock, type DockFilter } from "../../model/Dock";

export interface IDockService {
    getDocks(): Promise<Dock[]>;
    createDock(dock: Dock): Promise<Dock>;
    updateDock(code: string, dock: Dock): Promise<Dock>;
    filterDocks(filter: DockFilter): Promise<Dock[]>;
}