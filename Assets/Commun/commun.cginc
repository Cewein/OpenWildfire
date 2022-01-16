//commun library for shared code among the compute shader file

int flatten(int3 p, int size)
{
    p = clamp(p,int3(0,0,0), int3(size, size, size));
    return p.x + size * (p.y + size * p.z);
}

bool check(int3 id, int size)
{
    if (id.x < size && id.y < size && id.z < size) return true;
    return false;
}