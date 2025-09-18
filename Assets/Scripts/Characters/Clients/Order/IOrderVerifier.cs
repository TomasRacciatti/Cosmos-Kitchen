using Characters.Clients.Plates;
using Items.Core;

namespace Characters.Clients.Order
{
    public enum OrderOutcome { Wrong, Delivered, Perfect }
    
    public interface IOrderVerifier
    {
        OrderOutcome Verify(SoItem requestedPlate, ItemAmount delivered);
    }
    
    public sealed class PlainOrderVerifier : IOrderVerifier
    {
        public OrderOutcome Verify(SoItem requestedPlate, ItemAmount delivered)
        {
            if (requestedPlate != delivered.SoItem)
                return OrderOutcome.Wrong;

            return (delivered.TryGetStarRating(out var rating) && rating == StarRating.Gold)
                ? OrderOutcome.Perfect
                : OrderOutcome.Delivered;
        }
    }
}
