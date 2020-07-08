using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;

namespace FuzzySearchDemo.Models
{

    namespace SearchIndex
    {
        public struct ItemOfSearchResultInIndexByWord
        {
            public string SearchWord;
            public string IndexWord;
            public double Relevance;
            public double Probability;
            public int IndexInSearchIndex;
            public int NumberOfPage;
            public int WordIndexOnPage;
        };

        public struct ItemOfSummaryResultOfPage
        {
            public int NumberOfPage;
            public double Сoefficient;
            public List<SearchIndex.DataStringOfIndexWordsFoundOnPage> ListOfDataFoundIndexWords;
        };

        public struct DataStringOfIndexWordsFoundOnPage
        {
            public int PositionOnPage;
            public int WordLength;
        }

    }
    public partial class Search_Index_WarAndPeace
    {

        public string SearchString;
        public string[] Pages;
        public Models.SearchIndex.WordsIndexItem[] SearchWordsIndex;
        public SearchResult[] Results;
        public int NumberOfResults;

        public Search_Index_WarAndPeace(string[] PagesArray, Models.SearchIndex.WordsIndexItem[] InitSearchWordsIndex)
        {

            Pages = PagesArray;
            SearchWordsIndex = InitSearchWordsIndex;
            Results = new SearchResult[PagesArray.Length];
            SearchString = "";
            NumberOfResults = 0;

        }
        public void Search()
        {

            List<SearchIndex.ItemOfSearchResultInIndexByWord> PrimarySearchResults = new List<SearchIndex.ItemOfSearchResultInIndexByWord>();
            SearchIndex.ItemOfSearchResultInIndexByWord ItemOfSearchResultInIndexByWord;

            var WordsOfSearchStringList = GenerateWordsOfStringList(ref SearchString);
            string[] WordsOfSearchStringArray = WordsOfSearchStringList.ToArray<string>();

            FuzzySearchDemo.Models.Search SearchObj = new FuzzySearchDemo.Models.Search();
            SearchObj.Mode = 4;

            int NumberOfWordsInSearchStringIncludedInSearch = 0;
            double WordLengthRatio = 0.0;

            for (int i = 0; i < WordsOfSearchStringArray.Length; i++)
            {
                var WordOfSearchString = WordsOfSearchStringArray[i];
                if (WordOfSearchString.Length == 1)
                {
                    continue;
                }

                NumberOfWordsInSearchStringIncludedInSearch++;

                SearchObj.SearchString = WordOfSearchString;
                var InitialLetter = WordOfSearchString[0];
                
                for (int j = 0; j < SearchWordsIndex.Length; j++)
                {
                    if(InitialLetter != SearchWordsIndex[j].InitialLetterOfWord)
                    {
                        continue;
                    }

                    WordLengthRatio =
                               SearchWordsIndex[j].Word.Length > WordOfSearchString.Length ? (double)WordOfSearchString.Length / SearchWordsIndex[j].Word.Length
                                : (double)SearchWordsIndex[j].Word.Length / WordOfSearchString.Length;
                    if (WordLengthRatio < 0.5)
                    {
                        continue;
                    }

                    SearchObj.DataString = SearchWordsIndex[j].Word;
                    SearchObj.CalculateRelevance();
                    if(SearchObj.Relevance > 0.5)
                    {
                        int[] NumbersOfPages = GenerateNumbersOfPagesArray(ref SearchWordsIndex[j].PagesNumbers);
                        int[] WordIndexesOnPages = GenerateNumbersOfPagesArray(ref SearchWordsIndex[j].WordsIndexesOnPages);

                        for (int k = 0; k < NumbersOfPages.Length; k++)
                        {

                            ItemOfSearchResultInIndexByWord = new SearchIndex.ItemOfSearchResultInIndexByWord();

                            ItemOfSearchResultInIndexByWord.SearchWord = WordOfSearchString;
                            ItemOfSearchResultInIndexByWord.IndexWord = SearchWordsIndex[j].Word;
                            ItemOfSearchResultInIndexByWord.Relevance = SearchObj.Relevance;
                            ItemOfSearchResultInIndexByWord.Probability = SearchWordsIndex[j].Probability;
                            ItemOfSearchResultInIndexByWord.IndexInSearchIndex = j;
                            ItemOfSearchResultInIndexByWord.NumberOfPage = NumbersOfPages[k];
                            ItemOfSearchResultInIndexByWord.WordIndexOnPage = WordIndexesOnPages[k];

                            PrimarySearchResults.Add(ItemOfSearchResultInIndexByWord);
                        }
                    }
                }
            }

            List<SearchIndex.ItemOfSummaryResultOfPage> SummaryResultsOfPages = new List<SearchIndex.ItemOfSummaryResultOfPage>();

            SearchIndex.ItemOfSummaryResultOfPage ItemOfSummaryResultOfPage;


            int NumberOfCurrentPage = -1;
            string CurrentSearchWord = "";
            List<SearchIndex.DataStringOfIndexWordsFoundOnPage> DataOfWordsFoundOnPage = new List<SearchIndex.DataStringOfIndexWordsFoundOnPage>();
            SearchIndex.DataStringOfIndexWordsFoundOnPage DataStringOfIndexWordsFoundOnPage;
            
            double CurrentСoefficient = 0.0;

            PrimarySearchResults = PrimarySearchResults.OrderBy(x => x.NumberOfPage).ThenBy(x => x.SearchWord).ThenByDescending(x => x.Relevance).ToList();


            for (int i = 0; i < PrimarySearchResults.Count; i++)
            {

                if (i == 0)
                {

                    NumberOfCurrentPage = PrimarySearchResults[i].NumberOfPage;
                    CurrentSearchWord = PrimarySearchResults[i].SearchWord;

                    DataOfWordsFoundOnPage = new List<SearchIndex.DataStringOfIndexWordsFoundOnPage>();
                    DataStringOfIndexWordsFoundOnPage = new SearchIndex.DataStringOfIndexWordsFoundOnPage();
                    DataStringOfIndexWordsFoundOnPage.WordLength = PrimarySearchResults[i].IndexWord.Length;
                    DataStringOfIndexWordsFoundOnPage.PositionOnPage = PrimarySearchResults[i].WordIndexOnPage;
                    DataOfWordsFoundOnPage.Add(DataStringOfIndexWordsFoundOnPage);


                    CurrentСoefficient = 0.5 * PrimarySearchResults[i].Relevance + 0.5 * (1 - PrimarySearchResults[i].Probability);

                }
                else if (PrimarySearchResults[i].NumberOfPage != NumberOfCurrentPage)
                {

                    ItemOfSummaryResultOfPage = new SearchIndex.ItemOfSummaryResultOfPage();
                    ItemOfSummaryResultOfPage.NumberOfPage = NumberOfCurrentPage;
                    ItemOfSummaryResultOfPage.Сoefficient = CurrentСoefficient / NumberOfWordsInSearchStringIncludedInSearch;
                    ItemOfSummaryResultOfPage.ListOfDataFoundIndexWords = DataOfWordsFoundOnPage;
                    SummaryResultsOfPages.Add(ItemOfSummaryResultOfPage);

                    NumberOfCurrentPage = PrimarySearchResults[i].NumberOfPage;
                    CurrentSearchWord = PrimarySearchResults[i].SearchWord;

                    DataOfWordsFoundOnPage = new List<SearchIndex.DataStringOfIndexWordsFoundOnPage>();
                    DataStringOfIndexWordsFoundOnPage = new SearchIndex.DataStringOfIndexWordsFoundOnPage();
                    DataStringOfIndexWordsFoundOnPage.WordLength = PrimarySearchResults[i].IndexWord.Length;
                    DataStringOfIndexWordsFoundOnPage.PositionOnPage = PrimarySearchResults[i].WordIndexOnPage;
                    DataOfWordsFoundOnPage.Add(DataStringOfIndexWordsFoundOnPage);

                    CurrentСoefficient = 0.5 * PrimarySearchResults[i].Relevance + 0.5 * (1 - PrimarySearchResults[i].Probability);

                }
                else if(PrimarySearchResults[i].SearchWord != CurrentSearchWord)
                {

                    CurrentSearchWord = PrimarySearchResults[i].SearchWord;

                    CurrentСoefficient += 0.5 * PrimarySearchResults[i].Relevance + 0.5 * (1 - PrimarySearchResults[i].Probability);

                    DataStringOfIndexWordsFoundOnPage = new SearchIndex.DataStringOfIndexWordsFoundOnPage();
                    DataStringOfIndexWordsFoundOnPage.WordLength = PrimarySearchResults[i].IndexWord.Length;
                    DataStringOfIndexWordsFoundOnPage.PositionOnPage = PrimarySearchResults[i].WordIndexOnPage;
                    DataOfWordsFoundOnPage.Add(DataStringOfIndexWordsFoundOnPage);

                }
            }

            ItemOfSummaryResultOfPage = new SearchIndex.ItemOfSummaryResultOfPage();
            ItemOfSummaryResultOfPage.NumberOfPage = NumberOfCurrentPage;
            ItemOfSummaryResultOfPage.Сoefficient = CurrentСoefficient / NumberOfWordsInSearchStringIncludedInSearch;
            ItemOfSummaryResultOfPage.ListOfDataFoundIndexWords = DataOfWordsFoundOnPage;
            SummaryResultsOfPages.Add(ItemOfSummaryResultOfPage);

            SummaryResultsOfPages = SummaryResultsOfPages.OrderByDescending(x => x.Сoefficient).ToList();

            SearchObj.Mode = 1;
            SearchObj.SearchString = SearchString;

            Stack<int> BriefDisplayCharactersOfDataString = new Stack<int>();
            Stack<int> BriefFoundCharactersInDataString = new Stack<int>();
            Stack<int> BriefFoundCharactersInDataStringExtended = new Stack<int>();

            List<GroupsTableRow> ResultGroups = new List<GroupsTableRow>();

            NumberOfResults = 0;
            int NumberOfItems = SummaryResultsOfPages.Count > 100 ? 100 : SummaryResultsOfPages.Count;
            for (int i = 0; i < NumberOfItems; i++)
            {

                if (SummaryResultsOfPages[i].Сoefficient < 0.5)
                {
                    break;
                }

                BriefDisplayCharactersOfDataString.Clear();
                BriefFoundCharactersInDataString.Clear();
                BriefFoundCharactersInDataStringExtended.Clear();
                ResultGroups.Clear();

                for (var j = 0; j < SummaryResultsOfPages[i].ListOfDataFoundIndexWords.Count; j++)
                {
                    GroupsTableRow Group = new GroupsTableRow();
                    Group.GroupStartCoordinate = SummaryResultsOfPages[i].ListOfDataFoundIndexWords[j].PositionOnPage;
                    Group.GroupEndCoordinate = SummaryResultsOfPages[i].ListOfDataFoundIndexWords[j].PositionOnPage
                        + SummaryResultsOfPages[i].ListOfDataFoundIndexWords[j].WordLength - 1;
                    ResultGroups.Add(Group);
                }

                SearchObj.DataString = Pages[SummaryResultsOfPages[i].NumberOfPage - 1];
                SearchObj.FormRepresentationsOfResult(
                    ref ResultGroups,
                    ref BriefDisplayCharactersOfDataString,
                    ref BriefFoundCharactersInDataString,
                    ref BriefFoundCharactersInDataStringExtended,
                    false,
                    false);

                Results[NumberOfResults].Relevance = SummaryResultsOfPages[i].Сoefficient;
                Results[NumberOfResults].BriefDisplayOfFoundFragments = SearchObj.BriefDisplayOfFoundFragments;
                Results[NumberOfResults].InitialIndex = SummaryResultsOfPages[i].NumberOfPage - 1;

                NumberOfResults++;

            }

        }

        private List<string> GenerateWordsOfStringList(ref string SourceString)
        {

            List<string> WordsOfStringList = new List<string>();

            int StringBorder = SourceString.Length - 1;
            char SymbolOfRow;
            bool EmptySymbol;
            bool BodyOfWord = false;
            String TextOfCurrentWord = "";

            for (int i = 0; i <= StringBorder; i++)
            {

                SymbolOfRow = SourceString[i];
                EmptySymbol = SymbolOfRow == ' '
                    || ThisIsUselessSymbol(SymbolOfRow);

                if (BodyOfWord == false && EmptySymbol == false)
                {
                    BodyOfWord = true;
                    TextOfCurrentWord += SymbolOfRow;
                }
                else if (BodyOfWord == false && EmptySymbol == true)
                {

                }
                else if (BodyOfWord == true && EmptySymbol == false)
                {
                    TextOfCurrentWord += SymbolOfRow;
                }
                else if (BodyOfWord == true && EmptySymbol == true)
                {

                    WordsOfStringList.Add(TextOfCurrentWord.ToUpper());

                    BodyOfWord = false;
                    TextOfCurrentWord = "";

                }

            }

            if (BodyOfWord == true)
            {
                WordsOfStringList.Add(TextOfCurrentWord.ToUpper());
            }

            return WordsOfStringList;

        }

        private int[] GenerateNumbersOfPagesArray(ref string SourceString)
        {

            List<int> NumbersOfPagesList = new List<int>();

            int StringBorder = SourceString.Length - 1;
            char SymbolOfRow;
            bool EmptySymbol;
            bool BodyOfWord = false;
            String TextOfCurrentPage = "";

            for (int i = 0; i <= StringBorder; i++)
            {

                SymbolOfRow = SourceString[i];
                EmptySymbol = SymbolOfRow == ',';

                if (BodyOfWord == false && EmptySymbol == false)
                {
                    BodyOfWord = true;
                    TextOfCurrentPage += SymbolOfRow;
                }
                else if (BodyOfWord == false && EmptySymbol == true)
                {

                }
                else if (BodyOfWord == true && EmptySymbol == false)
                {
                    TextOfCurrentPage += SymbolOfRow;
                }
                else if (BodyOfWord == true && EmptySymbol == true)
                {

                    NumbersOfPagesList.Add(Convert.ToInt32(TextOfCurrentPage));

                    BodyOfWord = false;
                    TextOfCurrentPage = "";

                }

            }

            if (BodyOfWord == true)
            {
                NumbersOfPagesList.Add(Convert.ToInt32(TextOfCurrentPage));
            }

            return NumbersOfPagesList.ToArray();

        }

        private bool ThisIsUselessSymbol(char Simbol)
        {
            return Simbol == '.'
                || Simbol == ','
                || Simbol == ';'
                || Simbol == '('
                || Simbol == ')'
                || Simbol == '['
                || Simbol == ']'
                || Simbol == '/'
                || Simbol == '\\'
                || Simbol == '-'
                || Simbol == '–'
                || Simbol == '*'
                || Simbol == '»'
                || Simbol == '«'
                || Simbol == '\"'
                || Simbol == ':'
                || Simbol == '?'
                || Simbol == '!'
                || Simbol == '…'
                || Simbol == '\'';
        }

    }

}