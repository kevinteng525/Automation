require 'test_helper'

class TestSuitesControllerTest < ActionController::TestCase
  setup do
    @test_suite = test_suites(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:test_suites)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create test_suite" do
    assert_difference('TestSuite.count') do
      post :create, test_suite: { created_by: @test_suite.created_by, description: @test_suite.description, is_root: @test_suite.is_root, modified_by: @test_suite.modified_by, name: @test_suite.name, sub_suites: @test_suite.sub_suites, test_cases: @test_suite.test_cases }
    end

    assert_redirected_to test_suite_path(assigns(:test_suite))
  end

  test "should show test_suite" do
    get :show, id: @test_suite
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @test_suite
    assert_response :success
  end

  test "should update test_suite" do
    patch :update, id: @test_suite, test_suite: { created_by: @test_suite.created_by, description: @test_suite.description, is_root: @test_suite.is_root, modified_by: @test_suite.modified_by, name: @test_suite.name, sub_suites: @test_suite.sub_suites, test_cases: @test_suite.test_cases }
    assert_redirected_to test_suite_path(assigns(:test_suite))
  end

  test "should destroy test_suite" do
    assert_difference('TestSuite.count', -1) do
      delete :destroy, id: @test_suite
    end

    assert_redirected_to test_suites_path
  end
end
