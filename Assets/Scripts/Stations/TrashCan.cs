using UI.Trash;

namespace Stations
{
    public class TrashCan : Station
    {
        private TrashDropZone _dropZone;

        protected override void EnterStation()
        {
            base.EnterStation();
            if (CanvasInstance == null) return;

            _dropZone = CanvasInstance.GetComponentInChildren<TrashDropZone>(true);
            if (_dropZone != null) _dropZone.ItemTrashed += OnItemTrashed;
        }

        protected override void LeaveStation()
        {
            if (_dropZone != null)
            {
                _dropZone.ItemTrashed -= OnItemTrashed;
                _dropZone = null;
            }
            base.LeaveStation();
        }

        private void OnItemTrashed(TrashedItem trashed)
        {
            if (processingClip != null && audioSource != null)
                audioSource.PlayOneShot(processingClip);
        }
    }
}
