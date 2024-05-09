using System.Collections;
using Codewars.Training.PaginationHelper;

namespace Codewars.Training;

[TestFixture]
public class PaginationHelperTests
{
    [TestCaseSource(typeof(PageCountTestCases))]
    public void PageCountTest(PaginationHelper<char> helper, int expected)
    {
        Assert.That(helper.PageCount, Is.EqualTo(expected));
    }

    [TestCaseSource(typeof(ItemCountTestCases))]
    public void ItemCountTest(PaginationHelper<char> helper, int expected)
    {
        Assert.That(helper.ItemCount, Is.EqualTo(expected));
    }


    [TestCaseSource(typeof(PageItemCountTestCases))]
    public int PageItemCountTest(PaginationHelper<char> helper, int pageIndex)
    {
        return helper.PageItemCount(pageIndex);
    }

    [TestCaseSource(typeof(PageIndexTestCases))]
    public int PageIndexTest(PaginationHelper<char> helper, int itemIndex)
    {
        return helper.PageIndex(itemIndex);
    }
}

file sealed class PageCountTestCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), -1), -1)
            .SetName("EmptyCollectionAndNegativeItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 0), -1)
            .SetName("EmptyCollectionAndZeroItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 1), 0)
            .SetName("EmptyCollectionAndNItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 0), -1)
            .SetName("SingleItemAndZeroItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 1), 1)
            .SetName("SingleItemAndOneItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b',
            }, 1), 2)
            .SetName("TwoItemsAndOneItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
        {
            'a', 'b',
        }, 3), 1);
    }
}

file sealed class ItemCountTestCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 1), 0)
            .SetName("EmptyCollectionAndOneItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 1), 1)
            .SetName("SingleItemAndOneItemsPerPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b', 'c',
            }, 1), 3)
            .SetName("SingleItemAndOneItemsPerPage");
    }
}

file sealed class PageItemCountTestCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), -1), 0)
            .Returns(-1)
            .SetName("EmptyCollectionWithNegativeItemsPerPageFirstPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 0), 0)
            .Returns(-1)
            .SetName("EmptyCollectionWithZeroItemsPerPageFirstPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 1), 0)
            .Returns(-1)
            .SetName("EmptyCollectionWithNItemsPerPageFirstPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 0), 0)
            .Returns(-1)
            .SetName("SingleItemCollectionWithZeroItemsPerPageFirstPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 1), 0)
            .Returns(1)
            .SetName("SingleItemCollectionWithSingleItemsPerPageFirstPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 1), 1)
            .Returns(-1)
            .SetName("SingleItemCollectionWithSingleItemsPerPageSecondPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b', 'c',
            }, 2), 1)
            .Returns(1)
            .SetName("ThreeItemCollectionWithTwoItemsPerPageSecondPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b', 'c', 'd',
            }, 2), 1)
            .Returns(2)
            .SetName("FourItemCollectionWithTwoItemsPerPageSecondPage");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b', 'c', 'd',
            }, 2), -1)
            .Returns(-1)
            .SetName("FourItemCollectionWithTwoItemsPerPageNegativePage");
    }
}

file sealed class PageIndexTestCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), -1), -1)
            .Returns(-1)
            .SetName("EmptyCollectionWithNegativeItemsPerPageNegativeItemIndex");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), -1), 0)
            .Returns(-1)
            .SetName("EmptyCollectionWithNegativeItemsPerPageFirstItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 0), 0)
            .Returns(-1)
            .SetName("EmptyCollectionWithZeroItemsPerPageFirstItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 1), -1)
            .Returns(-1)
            .SetName("EmptyCollectionWithSingleItemsPerPageNegativeItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>(), 1), 0)
            .Returns(-1)
            .SetName("EmptyCollectionWithSingleItemsPerPageFirstItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 1), 0)
            .Returns(0)
            .SetName("SingleItemCollectionWithSingleItemsPerPageFirstItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a',
            }, 1), 1)
            .Returns(-1)
            .SetName("SingleItemCollectionWithSingleItemsPerPageSecondItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b',
            }, 1), 1)
            .Returns(1)
            .SetName("TwoItemCollectionWithSingleItemsPerPageSecondItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b',
            }, 2), 1)
            .Returns(0)
            .SetName("TwoItemCollectionWithTwoItemsPerPageSecondItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b', 'c',
            }, 2), 2)
            .Returns(1)
            .SetName("ThreeItemCollectionWithTwoItemsPerPageThirdItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b', 'c', 'd',
            }, 2), 2)
            .Returns(1)
            .SetName("FourItemCollectionWithTwoItemsPerPageThirdItem");
        yield return new TestCaseData(new PaginationHelper<char>(new List<char>
            {
                'a', 'b', 'c', 'd',
            }, 4), 2)
            .Returns(0)
            .SetName("FourItemCollectionWithFourItemsPerPageThirdItem");
    }
}