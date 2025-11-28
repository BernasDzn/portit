
import { chunkIndexToPosition, worldOrigin, chunkSize, LandChunk, WarehouseChunk, YardChunk, DockChunk } from "../chunk_layout.ts";

export function worldPosToChunk(pos) {
    const cx = Math.floor((pos.x - worldOrigin.x) / chunkSize.x);
    const cy = Math.floor((worldOrigin.z - pos.z) / chunkSize.z);
    return { x: cx, y: cy };
}

function buildNavGrid(chunkData) {
    const size = 10; // your world is 10x10
    const grid = Array.from({ length: size }, () => Array(size).fill(0));

    return grid;
}

function aStar(grid, start, goal) {
    const dirs = [
        [1,0], [-1,0], [0,1], [0,-1]
    ];

    const open = [];
    const cameFrom = {};

    const key = (x,y) => `${x},${y}`;

    open.push({ x:start.x, y:start.y, g:0, f:0 });

    const gScore = {};
    gScore[key(start.x,start.y)] = 0;

    while (open.length > 0) {
        open.sort((a,b) => a.f - b.f);
        const current = open.shift();

        if (current.x === goal.x && current.y === goal.y) {
            // reconstruct path
            const path = [];
            let cKey = key(current.x, current.y);

            while (cameFrom[cKey]) {
                const [cx, cy] = cKey.split(',').map(Number);
                path.push({ x: cx, y: cy });
                cKey = cameFrom[cKey];
            }
            return path.reverse();
        }

        for (const [dx, dy] of dirs) {
            const nx = current.x + dx;
            const ny = current.y + dy;

            if (nx < 0 || nx >= 10 || ny < 0 || ny >= 10) continue;
            if (grid[ny][nx] === 1) continue; // blocked

            const newG = gScore[key(current.x, current.y)] + 1;
            const neighborKey = key(nx, ny);

            if (!(neighborKey in gScore) || newG < gScore[neighborKey]) {
                gScore[neighborKey] = newG;
                cameFrom[neighborKey] = key(current.x, current.y);

                const h = Math.abs(nx - goal.x) + Math.abs(ny - goal.y);
                const f = newG + h;

                open.push({ x: nx, y: ny, g: newG, f });
            }
        }
    }

    return null; // no path found
}

function gridPathToWorld(path) {
    return path.map(p => 
        chunkIndexToPosition(p.x, p.y, true)
    );
}

function findVesselPath(startPosition, berthPosition, layout) {

    const start = worldPosToChunk(startPosition);
    const goal = worldPosToChunk(berthPosition);

    const navGrid = buildNavGrid(layout.chunkData);

    // A* on chunk grid
    const chunkPath = aStar(navGrid, start, goal);
    if (!chunkPath) {
        console.error("No valid path to dock!");
        return [
            startPosition,
            berthPosition
        ];
    }

    const worldPath = gridPathToWorld(chunkPath);
    console.log("Calculated vessel path:", worldPath);
    return [
        startPosition,
        berthPosition
    ];
}

export { findVesselPath };