using UnityEngine;
using Utils;

namespace Management.Audio
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField] private AudioSource gameAudioSource;
        
        [SerializeField] private AudioClip buttonPressSound;
        [SerializeField] private AudioClip tetrominoLandSound;
        [SerializeField] private AudioClip tetrisSound;
        [SerializeField] private AudioClip explosionSound;

        public void PressButton() => gameAudioSource.PlayOneShot(buttonPressSound);
        public void TetrominoLand() => gameAudioSource.PlayOneShot(tetrominoLandSound);
        public void RowClearing() => gameAudioSource.PlayOneShot(tetrisSound);
        public void Explode() => gameAudioSource.PlayOneShot(explosionSound);
    }
}