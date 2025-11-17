const ChunkType = {
	Land: 0,
    Dock: 1,
    Warehouse: 2,
    Yard: 3,
    STSCrane: 4,
    YardCrane: 5,
};

export async function fetchPortLayout() {
    try {
        const response = await fetch(`/api/PortLayout`);
        if (!response.ok) {
            console.error('Failed to fetch port layout:', response.statusText);
            return [];
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching port layout:', error);
        return [];
    }
}

export { ChunkType };
