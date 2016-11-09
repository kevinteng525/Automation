class TestDepotController < ApplicationController
  def index
    @root_test_suites = Array.new
    root_test_suite = TestSuite.get_root_test_suite
    if root_test_suite != nil      
      @root_test_suites.push(root_test_suite)
    end
    
    @rankings = Array.new
    TestCase.get_test_case_rankings().each do |ranking|
      @rankings.push(ranking)
    end
    @releases = Array.new
    TestCase.get_test_case_releases().each do |release|
      @releases.push(release)
    end
  end
  
  def user_created_test_suites_index
    @root_test_suites = Array.new
    root_test_suite = TestSuite.get_root_user_created_test_suite
    if root_test_suite != nil      
      @root_test_suites.push(root_test_suite)
    end
  end
  
  def test_suites_of_external_providers_index
    @root_test_suites = nil
    root_test_suite = TestSuite.get_root_rqm_test_suite
    if root_test_suite != nil
      @root_test_suites = Array.new
      @root_test_suites.push(root_test_suite)
    end
  end  

  def test_plans_of_external_providers_index
    @root_test_suites = Array.new
    root_test_suite = TestSuite.get_root_rqm_test_plans_suite
    if root_test_suite != nil      
      @root_test_suites.push(root_test_suite)
    end
  end
    
  def refreshtestcases
    @refresh_result = TestSuite.refresh_test_case_from_external_system
  end

  def refreshtestsuites
    @refresh_result = TestSuite.refresh_test_suite_from_external_system
  end
end
