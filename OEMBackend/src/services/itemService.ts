import { Item, items } from '../domain/item';

export class ItemService {
  getAll(): Item[] {
    return items;
  }

  getById(id: number): Item | undefined {
    return items.find((item) => item.id === id);
  }

  create(name: string): Item {
    const newItem: Item = {
      id: Date.now(),
      name,
    };
    items.push(newItem);
    return newItem;
  }

  update(id: number, name: string): Item | null {
    const itemIndex = items.findIndex((item) => item.id === id);
    if (itemIndex === -1) {
      return null;
    }
    const item = items[itemIndex];
    if (!item) {
      return null;
    }
    item.name = name;
    return item;
  }

  delete(id: number): Item | null {
    const itemIndex = items.findIndex((item) => item.id === id);
    if (itemIndex === -1) {
      return null;
    }
    const deletedItem = items.splice(itemIndex, 1)[0];
    return deletedItem || null;
  }
}

export const itemService = new ItemService();
