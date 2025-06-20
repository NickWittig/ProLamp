public enum NetworkExecutionType
{
    /// <summary>
    /// Executes only locally.
    /// </summary>
    Local,
    /// <summary>
    /// Executes locally and with Server RPC.
    /// </summary>
    LocalServer,
    /// <summary>
    /// Executes locally and with Client RPC.
    /// </summary>
    LocalClient,
    /// <summary>
    /// Executes locally, with Server- and with Client RPC.
    /// </summary>
    All
}