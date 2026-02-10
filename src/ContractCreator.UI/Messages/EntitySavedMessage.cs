namespace ContractCreator.UI.Messages
{
    public class EntitySavedMessage<T>
    {
        public T? Entity { get; }
        public bool IsNew { get; }

        public EntitySavedMessage(T? entity, bool isNew = false)
        {
            Entity = entity;
            IsNew = isNew;
        }
    }
}
