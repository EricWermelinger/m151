export interface RunDTO {
    id: string;
    title: string;
    startTime: Date;
    duration: number;
    length: number;
    altitude: number;
    gpxFileId: string | null;
}