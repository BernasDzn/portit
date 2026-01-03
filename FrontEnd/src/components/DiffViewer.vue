<script setup lang="ts">
import { computed } from 'vue';
import * as Diff from 'diff';

interface Props {
    oldText: string;
    newText: string;
    oldLabel?: string;
    newLabel?: string;
}

const props = withDefaults(defineProps<Props>(), {
    oldLabel: 'Previous Version',
    newLabel: 'Current Version'
});

const diffLines = computed(() => {
    return Diff.diffLines(props.oldText, props.newText);
});

const getLineNumbers = (change: Diff.Change, startOld: number, startNew: number) => {
    const lines = change.value.split('\n').filter((_, i, arr) => i < arr.length - 1 || change.value.endsWith('\n'));
    const oldNums: (number | null)[] = [];
    const newNums: (number | null)[] = [];
    
    if (change.added) {
        for (let i = 0; i < lines.length; i++) {
            oldNums.push(null);
            newNums.push(startNew + i);
        }
    } else if (change.removed) {
        for (let i = 0; i < lines.length; i++) {
            oldNums.push(startOld + i);
            newNums.push(null);
        }
    } else {
        for (let i = 0; i < lines.length; i++) {
            oldNums.push(startOld + i);
            newNums.push(startNew + i);
        }
    }
    
    return { oldNums, newNums, lines };
};

const formattedDiff = computed(() => {
    let oldLineNum = 1;
    let newLineNum = 1;
    const result: Array<{
        type: 'added' | 'removed' | 'unchanged';
        oldLineNum: number | null;
        newLineNum: number | null;
        content: string;
    }> = [];
    
    diffLines.value.forEach(change => {
        const { oldNums, newNums, lines } = getLineNumbers(change, oldLineNum, newLineNum);
        
        lines.forEach((line, i) => {
            result.push({
                type: change.added ? 'added' : change.removed ? 'removed' : 'unchanged',
                oldLineNum: oldNums[i],
                newLineNum: newNums[i],
                content: line
            });
        });
        
        if (!change.added) {
            oldLineNum += lines.length;
        }
        if (!change.removed) {
            newLineNum += lines.length;
        }
    });
    
    return result;
});
</script>

<template>
    <div class="diff-viewer">
        <div class="diff-header">
            <div class="diff-label old-label">{{ oldLabel }}</div>
            <div class="diff-label new-label">{{ newLabel }}</div>
        </div>
        <div class="diff-content">
            <table class="diff-table">
                <tbody>
                    <tr 
                        v-for="(line, index) in formattedDiff" 
                        :key="index"
                        :class="['diff-line', `diff-line-${line.type}`]"
                    >
                        <td class="line-number old-line-number">
                            {{ line.oldLineNum ?? '' }}
                        </td>
                        <td class="line-number new-line-number">
                            {{ line.newLineNum ?? '' }}
                        </td>
                        <td class="line-content">
                            <span class="line-marker" v-if="line.type === 'added'">+</span>
                            <span class="line-marker" v-else-if="line.type === 'removed'">-</span>
                            <span class="line-marker" v-else>&nbsp;</span>
                            <span class="line-text">{{ line.content }}</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<style scoped>
.diff-viewer {
    border: 1px solid var(--sl-color-neutral-300);
    border-radius: 6px;
    overflow: hidden;
    font-family: 'Courier New', monospace;
    font-size: 13px;
}

.diff-header {
    display: grid;
    grid-template-columns: 1fr 1fr;
    background-color: var(--sl-color-neutral-100);
    border-bottom: 1px solid var(--sl-color-neutral-300);
}

.diff-label {
    padding: 8px 12px;
    font-weight: 600;
    text-align: center;
}

.old-label {
    border-right: 1px solid var(--sl-color-neutral-300);
    color: var(--sl-color-danger-600);
}

.new-label {
    color: var(--sl-color-success-600);
}

.diff-content {
    overflow-x: auto;
    background-color: white;
    max-height: 600px;
    overflow-y: auto;
}

.diff-table {
    width: 100%;
    border-collapse: collapse;
}

.diff-line {
    line-height: 1.5;
}

.line-number {
    padding: 2px 8px;
    text-align: right;
    user-select: none;
    vertical-align: top;
    min-width: 40px;
    background-color: var(--sl-color-neutral-50);
    color: var(--sl-color-neutral-500);
    border-right: 1px solid var(--sl-color-neutral-200);
}

.new-line-number {
    border-right: 1px solid var(--sl-color-neutral-300);
}

.line-content {
    padding: 2px 8px;
    white-space: pre-wrap;
    word-break: break-word;
    vertical-align: top;
}

.line-marker {
    display: inline-block;
    width: 20px;
    font-weight: bold;
}

.diff-line-added {
    background-color: #e6ffec;
}

.diff-line-added .line-marker {
    color: #22863a;
}

.diff-line-added .line-number {
    background-color: #cdffd8;
}

.diff-line-removed {
    background-color: #ffebe9;
}

.diff-line-removed .line-marker {
    color: #cb2431;
}

.diff-line-removed .line-number {
    background-color: #ffdce0;
}

.diff-line-unchanged {
    background-color: white;
}

.line-text {
    white-space: pre-wrap;
}
</style>
