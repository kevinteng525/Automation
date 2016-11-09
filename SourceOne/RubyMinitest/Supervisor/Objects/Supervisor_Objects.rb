require 'watir-webdriver'
require 'watir-webdriver-performance'
gem "minitest"
require "minitest/autorun"

#root browser class.
class BrowserContainer
  def initialize(browser)
    @browser = browser
  end

  #this is a function of close browser.
  def close
    @browser.close
  end
  
  #this is a function of record the browser loading time.
  def load_time(default_sec=10, webname="web")
    _loadtime = @browser.performance.summary[:response_time]/1000
    if _loadtime > default_sec
      puts "error: The "+webname+" page Load Time: #{_loadtime} seconds. It is too slow!!! The expected load time is less than "+default_sec.to_s+" seconds."
    end
  end    
end

class SUPSite < BrowserContainer
    
  #This is a page of login.
  def SUPlogin_page
    @SUPlogin_page = SUPLoginPage.new(@browser)
  end

  #This is a page of Filter Messages dialog box.
  def SUPfilter_message_page
    @SUPfilter_message_page = SUPFilterMessagePage.new(@browser)
  end
  
  #This is a header of main page.
  def SUPheader_page
    @SUPheader_page = SUPHeaderPage.new(@browser)
  end

  #This is a reviewer messages list page.
  def SUPMessageList_page
    @SUPMessageList_page = SUPMessageListPage.new(@browser)
  end

  #This is a BP pane.
  def SUPBusinessPolicy_Pane
    @SUPBusinessPolicy_Pane = SUPBusinessPolicyPane.new(@browser)
  end
  
  #This is a preview pane.
  def SUPPreview_Pane
    @SUPPreview_Pane = SUPPreviewPane.new(@browser)
  end
  
  #This is a report page.
  def SUPReport_Page
    @SUPReport_Page = SUPReportPage.new(@browser)
  end  
  
end 
