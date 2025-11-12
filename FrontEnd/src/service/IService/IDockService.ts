import type { Dock } from "@/model/Dock";
import type { DockDto } from "@/model/dto/DockDto";
import type { Filter, Page } from "@/model/Page";

export interface IDockService {
    getDocks(filtering?: Filter<Dock>): Promise<Page<Dock>>
    getDockByCode(code: string): Promise<Dock | undefined>;

    createDock(dock: DockDto): Promise<Dock>;
    updateDock(dock: DockDto): Promise<Dock>;
    
    getNumberOfDocks(): Promise<number>;
}