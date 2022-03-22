export interface RunFilterDTO {
    lengthMin: number;
    lengthMax: number;
    altitudeMin: number;
    altitudeMax: number;
    pointLatitude: number | null;
    pointLongitude: number | null;
    radiusFromPoint: number | null;
}