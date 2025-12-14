export default class LinkedList<T> {
	private head: Node<T> | null = null;

	public insertInBegin(data: T): Node<T> {
		const node = new Node(data);
		if (!this.head) {
			this.head = node;
		} else {
			this.head.prev = node;
			node.next = this.head;
			this.head = node;
		}
		return node;
	}

	public insertAtEnd(data: T): Node<T> {
		const node = new Node(data);
		if (!this.head) {
			this.head = node;
		} else {
			const getLast = (node: Node<T>): Node<T> => {
				return node.next ? getLast(node.next) : node;
			};
			const lastNode = getLast(this.head);
			node.prev = lastNode;
			lastNode.next = node;
		}
		return node;
	}

	public deleteNode(node: Node<T>): void {
		if (!node.prev) {
		this.head = node.next;
		} else {
		const prevNode = node.prev;
		prevNode.next = node.next;
		}
	}

	public size(): number {
		let count = 0;
		let current = this.head;
		while (current) {
			count++;
			current = current.next;
		}
		return count;
	}

	public search(comparator: (data: T) => boolean): Node<T> | null {
		let current = this.head;
		while (current) {
			if (comparator(current.data)) {
				return current;
			}
			current = current.next;
		}
		return null;
	}

	public toArray(): T[] {
		const array: T[] = [];
		let current = this.head;
		while (current) {
			array.push(current.data);
			current = current.next;
		}
		return array;
	}
}

class Node<T> {
	public data: T;
	public next: Node<T> | null = null;
	public prev: Node<T> | null = null;

	constructor(data: T) { 
		this.data = data;
	}
}