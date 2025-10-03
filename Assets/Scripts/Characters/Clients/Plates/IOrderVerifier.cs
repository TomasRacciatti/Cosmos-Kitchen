using Items.Core;

namespace Characters.Clients.Plates
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

            return delivered.Rating == 3 ? OrderOutcome.Perfect : OrderOutcome.Delivered;
        }
    }
}
