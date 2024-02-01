using System.Collections;
using System.Collections.Generic;
using echo17.EndlessBook;
using UnityEngine;

public class BookController : MonoBehaviour
{
    public EndlessBook book;
    
    void Update()
    {
        InteractWithBook();
    }

    public void InteractWithBook()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(book.CurrentState == EndlessBook.StateEnum.ClosedFront)
            {
                book.SetState(EndlessBook.StateEnum.OpenMiddle);
            }
            else
            {
                book.SetState(EndlessBook.StateEnum.ClosedFront);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !book.IsFirstPageGroup)
        {
            book.TurnToPage(book.CurrentLeftPageNumber - 2, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !book.IsFirstPageGroup)
        {
            book.TurnToPage(book.CurrentLeftPageNumber + 2, EndlessBook.PageTurnTimeTypeEnum.TimePerPage, 1f);
        }
    }
}
