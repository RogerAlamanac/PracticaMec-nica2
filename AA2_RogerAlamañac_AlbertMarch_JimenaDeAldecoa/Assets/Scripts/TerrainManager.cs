public static class TerrainManager
{
    // Devuelve el coeficiente de fricción según tipo de terreno
    public static float GetFriction(int type)
    {
        switch (type)
        {
            case 0: return 0.4f; // grass
            case 1: return 0.1f; // ice
            case 2: return 0.6f; // sand
            default: return 0.3f;
        }
    }
}
