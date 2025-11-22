import type { Dock } from "@/model/Dock";
import type { DockDto } from "@/model/dto/DockDto";
import type { Filter, Page } from "@/model/Page";

export interface IDockService {
    getDocks(filtering?: Filter<Dock>): Promise<Page<Dock>>
    getDockByCode(code: string): Promise<Dock | undefined>;

    createDock(dock: Dock): Promise<Dock>;
    updateDock(dock: Dock): Promise<Dock>;
    
    getNumberOfDocks(): Promise<number>;
}