import { type Dock } from "../../model/Dock";

export interface IDockService {
    getDocks(): Promise<Dock[]>;
    
    // missing create, update, filter methods
}