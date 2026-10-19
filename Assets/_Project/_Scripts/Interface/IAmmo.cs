public interface IAmmo
{
    public int GetAmmo();
    public int GetMaxAmmo();
    public int GetMaxAmmoPerClip();
    public string GetAmmoText() => $"{GetAmmo()}/{GetMaxAmmo()}";
}