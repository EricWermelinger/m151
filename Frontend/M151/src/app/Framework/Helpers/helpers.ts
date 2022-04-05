export function FormatSeconds(seconds: number): string {
    return new Date(seconds * 1000).toISOString().substr(11, 8);
}

export function RandomColor(): string {
    return '#' + Math.floor(Math.random() * 16777215).toString(16);
}