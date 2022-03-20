import { GpxNodeDTO } from "./GpxNodeDTO";

export interface GpxFileDTO {
  runId: string;
  filename: string;
  nodes: GpxNodeDTO[];
}