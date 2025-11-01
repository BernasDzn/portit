import type { Filter, Page } from "@/model/Page";
import { type Dock, type DockFilter } from "../../model/Dock";

export interface IDockService {
    getDocks(filtering: Filter<Dock>): Promise<Page<Dock>>
    createDock(dock: Dock): Promise<Dock>;
    updateDock(code: string, dock: Dock): Promise<Dock>;
}