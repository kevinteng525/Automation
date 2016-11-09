module TestSuitesHelper
  def suites_list
    [['suite1',1]]
  end
  def test_cases_list
    TestCase.all.collect{|tc|[tc.name, tc.id]}
  end
  
  def test_suite_type_list
    TestSuite::SUITE_TYPES.collect{|t|[t,TestSuite::SUITE_TYPES.index(t)]}
  end
end
