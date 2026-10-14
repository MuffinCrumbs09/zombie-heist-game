public interface IAmmo
{
    int GetAmmo();
    int GetMaxAmmo();
    int GetMaxAmmoPerClip();
    string GetAmmoText() => $"{GetAmmo()}/{GetMaxAmmo()}";
}