require 'watir-webdriver'
require_relative '..\Variables\Variables_Configure.rb'
require_relative '..\Objects\DM_Objects.rb'

gem "minitest"
require "minitest/autorun"

class SearchSummaryPage < BrowserContainer
  
  #Verify whether the search name is created.
  def validSearchName(sname)
    if search_name_text.exist?
    search_name_text.text.include? sname
    else
      return false
    end
  end
  
  #Verify whethre the search name is in the grid
  def validSearchNameGrid(sname)
    @browser.div(:class=>'gridxBody gridxBodyRowHoverEffect').span(:title=>sname).exist?
  end
  
  #Verify which the folder the search in 
  def validSearchFolder(folder)
    if search_name_folder(folder).exist?
      return true
    else
      return false
    end
  end
  

  #Verify whether the search is run completed.
  def verifySearchStatus
    search_status_text.text.include? 'Complete'
  end
  
  private
  
  #Collection Search: Search name.
  def search_name_text
    @browser.span(:id => 'search-summary-search-name')
  end
  
  
  #search folder
  def search_name_folder(folder)
    @browser.div(:class=>'gridxMain').span(:title =>folder)
  end
  
  #Search Status.
  def search_status_text
    @browser.span(:id => 'search-summary-status')
  end
  
end

