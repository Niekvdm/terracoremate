using TerracoreMate.Models;

namespace TerracoreMate.Handlers.Interfaces;

public interface IHandler
{    
    /// <summary>
    /// Sets the class-level account to the provided account.
    /// </summary>
    /// <param name="account">The account to be set in the class context</param>
    public void SetAccount(Account account);
        
    /// <summary>
    /// Trigger the main action of the handler. The specific behavior depends on the implementation of this interface.
    /// </summary>
    /// <returns>The task instance representing the asynchronous operation.</returns>
    public Task Fire();
}