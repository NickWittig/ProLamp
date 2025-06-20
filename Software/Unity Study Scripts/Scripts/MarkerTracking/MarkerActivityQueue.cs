using System.Collections.Generic;

public class MarkerActivityQueue
{
    private Queue<short> queue;
    private int queueSize;

    public MarkerActivityQueue(int size)
    {
        queueSize = size;
        queue = new Queue<short>(queueSize);

        // Initialize queue with -1 values
        for (int i = 0; i < queueSize; i++)
        {
            queue.Enqueue(-1);
        }
    }

    // Method to add IDs to the queue
    public void EnqueueID(short id)
    {
        // Dequeue the first element if the queue is at capacity
        if (queue.Count == queueSize)
        {
            queue.Dequeue();
        }

        // Enqueue the new ID
        queue.Enqueue(id);
    }

    // Method to check if the queue contains only -1 values
    public bool IsMarkerActive()
    {
        // Check if the queue contains all -1 values
        foreach (short value in queue)
        {
            if (value != -1)
            {
                return true;
            }
        }

        return false;
    }
}
