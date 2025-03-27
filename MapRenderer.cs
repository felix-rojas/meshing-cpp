namespace meshing;

public unsafe partial class MapRenderer
{
    public Map m;

    public float cameraPitch;
    public float cameraYaw;

 public MapRenderer(int max_width, int max_height, int max_depth)
{
    // Create and configure FastNoise object
    FastNoiseLite noise = new();
    noise.SetNoiseType(FastNoiseLite.NoiseType.ValueCubic);
    noise.SetFrequency(0.02f); // Adjust frequency for finer or coarser details

    m = new Map(max_width, max_height, max_depth);

    for (int x = 0; x < max_width; x++)
    {
        for (int z = 0; z < max_depth; z++)
        {
            // Generate noise-based height value
            float noiseValue = noise.GetNoise(x, z);
            int height = (int)((noiseValue + 1) * 0.5f * max_height); // Normalize noise to fit height range

            for (int y = 0; y < height && y < max_height; y++)
            {
                m.AddVoxel(z, y, x, 1);
            }
        }
    }
}


    public void Render()
    { 
        // Determine chunk rendering order for best front-to-back rendering on the GPU
        int startX, endX, xStep, startZ, endZ, zStep;
        int xAccessStep, zAccessStep;
        var lookAt = Helper.FromPitchYaw(cameraPitch, cameraYaw);

        if (lookAt.X > 0)
        {
            startX = 0;
            endX = m.ChunkAmountX;
            xStep = 1;
            xAccessStep = Constants.ChunkSize;
        }
        else
        {
            startX = Math.Max(0, m.ChunkAmountX - 1);
            endX = -1;
            xStep = -1;
            xAccessStep = -Constants.ChunkSize;
        }

        if (lookAt.Z > 0)
        {
            startZ = 0;
            endZ = m.ChunkAmountZ;
            zStep = 1;
            zAccessStep = Constants.ChunkSizeSquared;
        }
        else
        {
            startZ = Math.Max(0, m.ChunkAmountZ - 1);
            endZ = -1;
            zStep = -1;
            zAccessStep = -Constants.ChunkSizeSquared;
        }



        // Loop over every chunk in the current range
        //  To avoid excessive multiplications in the inner-mode loop, chunkAccessX and chunkAccessZ are precalculated in the outer loops
        //  These access variables are needed because m.meshChunks is a 1-dimensional array
        var chunkAccessZ = startZ * Constants.ChunkSizeSquared;

        for (int k = startZ; k != endZ; k += zStep, chunkAccessZ += zAccessStep)
        {
            var chunkAccessX = startX * Constants.ChunkSize;

            for (int i = startX; i != endX; i += xStep, chunkAccessX += xAccessStep)
            {
                var chunkAccess = chunkAccessZ + chunkAccessX;

                for (int j = 0; j < m.ChunkAmountY; j++)
                {
                    var c = m.meshChunks[chunkAccess++];


                    // Ignore empty chunks
                    if (c == null)
                        continue;


                    // Mesh the chunk (if it's dirty)
                    MeshChunk(c);


                    // Render the chunk (if meshed successfully)
                    if (c.voxelBuffer != null)
                    {
                        VoxelShader.worldPosition.Set3(c.WorldPos);
                        c.voxelBuffer.BindAndDraw();
                    }
                }
            }
        }
    }

    protected void MeshChunk(ChunkMesh c)
    {
        // If not dirty, don't mesh
        if (!c.chunk->IsDirty())
            return;


        // Ensure we're a valid chunk
        Assert(!c.chunk->fake);


        // Mesh it
        c.PreMeshing();
        c.GenerateMesh();
        c.PostMeshing();
    }
}
