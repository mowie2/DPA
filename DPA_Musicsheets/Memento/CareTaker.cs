using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Memento
{
    class CareTaker
    {
        private Memento currentMemento;
        public bool canUndo { get; set; }
        public bool canRedo { get; set; }
        public CareTaker()
        {
            canUndo = false;
            canRedo = false;
        }

        public void AddMemento(Memento newMemento)
        {
            if(currentMemento == null)
            {
                currentMemento = newMemento;
            }
            else
            {
                currentMemento.Next = newMemento;
                newMemento.Previous = currentMemento;
                currentMemento = currentMemento.Next;
                canUndo = true;
                canRedo = false;
            }


        }

        public Memento GetCurrentMemento()
        {
            return currentMemento;
        }

        public void Undo()
        {
            if(currentMemento.Previous != null)
            {
                currentMemento = currentMemento.Previous;
            }

            if (currentMemento.Previous == null)
            {
                canUndo = false;
            }
            else
            {
                canUndo = true;
            }
            if (currentMemento.Next == null)
            {
                canRedo = false;
            }
            else
            {
                canRedo = true;
            }
        }

        public void Redo()
        {
            if (currentMemento.Next != null)
            {
                currentMemento = currentMemento.Next;
            }

            if (currentMemento.Previous == null)
            {
                canUndo = false;
            }
            else
            {
                canUndo = true;
            }

            if (currentMemento.Next == null)
            {
                canRedo = false;
            }
            else
            {
                canRedo = true;
            }
        }
    }
}
